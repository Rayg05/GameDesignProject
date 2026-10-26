using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// One-time tool: makes Player.prefab the single source of truth for the player and puts that exact
// player in every scene. Run it from Tools > Unify Player Prefab, then this file can be deleted.
public static class UnifyPlayerPrefab
{
    const string PrefabPath = "Assets/Prefab/Player.prefab";
    const string SourceScene = "Assets/Scenes/Boss Scenes/NeptuneBoss.unity"; // most complete player, gets applied to the prefab
    const string HubScene = "Assets/Scenes/HubWorld.unity";                    // has a standalone (non-prefab) player
    const string HudPrefabPath = "Assets/Prefab/HUD.prefab";                   // health bar + inventory, missing from the hub
    const string StrengthItemPath = "Assets/GameEquipment/StrengthItem.asset"; // no player had the StrengthItem component yet
    static readonly string[] InstanceScenes =
    {
        "Assets/Scenes/Boss Scenes/UranusBoss.unity",
        "Assets/Scenes/CurrentGame.unity",
        "Assets/Scenes/Scene2.unity",
    };

    [MenuItem("Tools/Unify Player Prefab")]
    static void Run()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (!EditorUtility.DisplayDialog("Unify Player Prefab",
            "This applies NeptuneBoss's player to Player.prefab, then replaces the player in HubWorld, " +
            "UranusBoss, CurrentGame and Scene2 with that prefab, adds the HUD/inventory to HubWorld " +
            "and saves every scene.\n\n" +
            "Make sure your work is committed in git first so you can undo it.", "Run", "Cancel")) return;

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);

        // 1. Neptune's player becomes the prefab
        Scene scene = EditorSceneManager.OpenScene(SourceScene, OpenSceneMode.Single);
        GameObject neptunePlayer = FindPlayerInstance(scene);
        if (neptunePlayer == null) { Debug.LogError("[Unify] No Player.prefab instance in NeptuneBoss, nothing changed"); return; }
        foreach (Transform t in neptunePlayer.GetComponentsInChildren<Transform>(true))
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
        PrefabUtility.ApplyPrefabInstance(neptunePlayer, InteractionMode.AutomatedAction);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Unify] Applied NeptuneBoss player to Player.prefab");
        AddStrengthItem();

        // 2. Hub: swap the standalone player for the prefab, keeping its InteractionDetector (added to the prefab)
        scene = EditorSceneManager.OpenScene(HubScene, OpenSceneMode.Single);
        GameObject hubPlayer = FindStandalonePlayer(scene);
        if (hubPlayer == null) { Debug.LogError("[Unify] No standalone player in HubWorld, stopped after updating the prefab"); return; }
        float oldHeight = ColliderHeight(hubPlayer);
        GameObject newHubPlayer = ReplaceWithPrefab(scene, hubPlayer, prefab, true);

        Transform detector = hubPlayer.transform.Find("InteractionDetector");
        if (detector != null)
        {
            detector.SetParent(newHubPlayer.transform, false);
            PrefabUtility.ApplyAddedGameObject(detector.gameObject, PrefabPath, InteractionMode.AutomatedAction);
        }
        Relink(scene, hubPlayer, newHubPlayer);
        Object.DestroyImmediate(hubPlayer);
        AddHud(scene, newHubPlayer);

        // zoom the hub camera out so the bigger player is framed like the old one
        float zoom = ColliderHeight(newHubPlayer) / oldHeight;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Camera cam in root.GetComponentsInChildren<Camera>(true))
            {
                if (!cam.orthographic) continue;
                cam.orthographicSize *= zoom;
                EditorUtility.SetDirty(cam);
                Debug.Log($"[Unify] HubWorld camera '{cam.name}' orthographic size -> {cam.orthographicSize:0.##}");
            }
        }
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Unify] HubWorld player replaced with Player.prefab");

        // 3. Other scenes: replace their (possibly modified) instance with a clean one
        foreach (string path in InstanceScenes)
        {
            scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            GameObject oldPlayer = FindPlayerInstance(scene);
            if (oldPlayer == null) { Debug.LogWarning($"[Unify] No player found in {path}, skipped"); continue; }
            GameObject newPlayer = ReplaceWithPrefab(scene, oldPlayer, prefab, false);
            Relink(scene, oldPlayer, newPlayer);
            Object.DestroyImmediate(oldPlayer);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[Unify] {path} player replaced with Player.prefab");
        }

        EditorSceneManager.OpenScene(SourceScene, OpenSceneMode.Single);
        EditorUtility.DisplayDialog("Unify Player Prefab", "Done. Check the Console for any [Unify] warnings.", "OK");
    }

    // Gives the prefab a StrengthItem wired to the side jab, so equipping Strength does something.
    static void AddStrengthItem()
    {
        GameObject contents = PrefabUtility.LoadPrefabContents(PrefabPath);
        if (contents.GetComponent<StrengthItem>() == null)
        {
            Transform sideJab = contents.transform.Find("Side Jab");
            JabAttackArea area = sideJab != null ? sideJab.GetComponent<JabAttackArea>() : contents.GetComponentInChildren<JabAttackArea>(true);

            var so = new SerializedObject(contents.AddComponent<StrengthItem>());
            so.FindProperty("jabAttack").objectReferenceValue = area;
            so.FindProperty("strengthItem").objectReferenceValue = AssetDatabase.LoadAssetAtPath<EquipmentData>(StrengthItemPath);
            so.ApplyModifiedPropertiesWithoutUndo();
            if (area == null) Debug.LogWarning("[Unify] Added StrengthItem but found no JabAttackArea for it, assign 'Jab Attack' by hand");
            else Debug.Log("[Unify] Added StrengthItem to Player.prefab");
        }
        PrefabUtility.SaveAsPrefabAsset(contents, PrefabPath);
        PrefabUtility.UnloadPrefabContents(contents);
    }

    // Puts the HUD (health bar + inventory) in the scene if it has none and points it at the player's health.
    static void AddHud(Scene scene, GameObject player)
    {
        HUD hud = null;
        foreach (GameObject root in scene.GetRootGameObjects())
            if (hud == null) hud = root.GetComponentInChildren<HUD>(true);

        if (hud == null)
        {
            var hudObject = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(HudPrefabPath), scene);
            hud = hudObject.GetComponentInChildren<HUD>(true);
            Debug.Log($"[Unify] Added HUD to {scene.name}");
        }

        var so = new SerializedObject(hud);
        so.FindProperty("playerHealth").objectReferenceValue = player.GetComponent<Health>();
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // Puts a fresh prefab instance where oldPlayer is. Call Relink afterwards to move scene references over.
    // alignFeet keeps the bottom of the collider at the same height when the player size changes.
    static GameObject ReplaceWithPrefab(Scene scene, GameObject oldPlayer, GameObject prefab, bool alignFeet)
    {
        GameObject newPlayer = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
        newPlayer.name = oldPlayer.name;
        newPlayer.transform.SetParent(oldPlayer.transform.parent, false);
        newPlayer.transform.SetSiblingIndex(oldPlayer.transform.GetSiblingIndex());
        newPlayer.transform.SetPositionAndRotation(oldPlayer.transform.position, oldPlayer.transform.rotation);

        if (alignFeet)
        {
            Vector3 pos = newPlayer.transform.position;
            pos.y += ColliderBottom(oldPlayer) - ColliderBottom(newPlayer);
            newPlayer.transform.position = pos;
        }
        return newPlayer;
    }

    // Any reference outside the player that pointed into the old player is moved to the same path/type in the new one.
    static void Relink(Scene scene, GameObject oldRoot, GameObject newRoot)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Component comp in root.GetComponentsInChildren<Component>(true))
            {
                if (comp == null) continue;
                if (comp.transform.IsChildOf(oldRoot.transform) || comp.transform.IsChildOf(newRoot.transform)) continue;

                var so = new SerializedObject(comp);
                SerializedProperty it = so.GetIterator();
                bool changed = false;
                while (it.Next(true))
                {
                    if (it.propertyType != SerializedPropertyType.ObjectReference || it.objectReferenceValue == null) continue;

                    Object target = it.objectReferenceValue;
                    GameObject go = target as GameObject ?? (target as Component)?.gameObject;
                    if (go == null || !go.transform.IsChildOf(oldRoot.transform)) continue;

                    string path = AnimationUtility.CalculateTransformPath(go.transform, oldRoot.transform);
                    Transform match = path == "" ? newRoot.transform : newRoot.transform.Find(path);
                    Object replacement = null;
                    if (match != null)
                        replacement = target is GameObject ? match.gameObject : match.GetComponent(target.GetType());

                    it.objectReferenceValue = replacement;
                    changed = true;
                    if (replacement == null)
                        Debug.LogWarning($"[Unify] {scene.name}: {comp.GetType().Name} on '{comp.name}' pointed at '{path}' " +
                                         $"({target.GetType().Name}), which the prefab doesn't have. Reassign it by hand.", comp);
                }
                if (changed) so.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }

    static GameObject FindPlayerInstance(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
                if (PrefabUtility.IsOutermostPrefabInstanceRoot(t.gameObject) &&
                    PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(t.gameObject) == PrefabPath)
                    return t.gameObject;
        return null;
    }

    static GameObject FindStandalonePlayer(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Basic_Movement2 mover in root.GetComponentsInChildren<Basic_Movement2>(true))
                if (!PrefabUtility.IsPartOfPrefabInstance(mover)) return mover.gameObject;
        return null;
    }

    static float ColliderHeight(GameObject player)
    {
        var box = player.GetComponent<BoxCollider2D>();
        return box.size.y * Mathf.Abs(player.transform.lossyScale.y);
    }

    static float ColliderBottom(GameObject player)
    {
        var box = player.GetComponent<BoxCollider2D>();
        float scaleY = Mathf.Abs(player.transform.lossyScale.y);
        return player.transform.position.y + (box.offset.y - box.size.y * 0.5f) * scaleY;
    }
}
