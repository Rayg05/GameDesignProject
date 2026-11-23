using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawn_Minions : MonoBehaviour
{
    [SerializeField] Transform[] anchors;
    [SerializeField] Minion minionPrefab;
    [SerializeField] Transform player;
    [SerializeField] int minMinions = 4;
    [SerializeField] int maxMinions = 5;
    [SerializeField] float spawnInterval = 1.5f;         // time to wait after each prefab spawns
    [SerializeField] float recoveryTime = 1f;
    [SerializeField] float minDistanceFromPlayer = 8f;   // anchors closer than this (on x) won't be used

    public bool IsRunning { get; private set; }

    public void StartAttack()
    {
        if (IsRunning) return;
        if (anchors == null || anchors.Length <= 0)
        {
            Debug.LogError("no anchors assigned");
            return;
        }
        StartCoroutine(RunAttack());
    }

    IEnumerator RunAttack()
    {
        IsRunning = true;

        int count = Random.Range(minMinions, maxMinions + 1);
        foreach (var anchor in PickAnchors(count))
        {
            Instantiate(minionPrefab, anchor.position, Quaternion.Euler(0f, 0f, 180f));
            yield return new WaitForSeconds(spawnInterval);
        }

        // give the last piece time to fall before handing control back
        //yield return new WaitForSeconds(recoveryTime);
        IsRunning = false;
    }

    List<Transform> PickAnchors(int count)
    {
        var candidates = new List<Transform>();
        foreach (Transform anchor in anchors)
        {
            float distanceToPlayer = Mathf.Abs(anchor.position.x - player.position.x);

            if (distanceToPlayer >= minDistanceFromPlayer)
                candidates.Add(anchor);
        }

        var picks = new List<Transform>();
        while (picks.Count < count && candidates.Count > 0)
        {
            int randomIndex = Random.Range(0, candidates.Count);
            picks.Add(candidates[randomIndex]);
            candidates.RemoveAt(randomIndex);
        }

        return picks;
    }
}