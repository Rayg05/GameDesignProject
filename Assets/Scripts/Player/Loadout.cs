using UnityEngine;

public class Loadout : MonoBehaviour
{
    // The main active slot that holds your selected prefab
    [SerializeField] private GameObject activePrefab;

    // A collection of prefabs you drag into the Inspector beforehand
    [SerializeField] private GameObject weaponPrefabA;
    [SerializeField] private GameObject weaponPrefabB;

    void Update()
    {
        // Change the active reference depending on user input
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            activePrefab = weaponPrefabA;
            Debug.Log("Switched active slot to Prefab A!");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            activePrefab = weaponPrefabB;
            Debug.Log("Switched active slot to Prefab B!");
        }

        // Example: Press Space to spawn whatever prefab is currently held
        if (Input.GetKeyDown(KeyCode.Space) && activePrefab != null)
        {
            Instantiate(activePrefab, transform.position, Quaternion.identity);
        }
    }
}