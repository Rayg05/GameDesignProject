using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CeilingAttackController : MonoBehaviour
{
    [SerializeField] Transform[] anchors;
    [SerializeField] FallingDebris debrisPrefab;
    [SerializeField] Transform player;
    [SerializeField] int minPieces = 4;
    [SerializeField] int maxPieces = 5;
    [SerializeField] Vector2 delayBetweenDrops = new Vector2(0.1f, 0.35f);
    [SerializeField] float recoveryTime = 1f;
    [SerializeField] float maxDistanceFromPlayer = 8f;   // anchors farther than this (on x) won't be used

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

        int count = Random.Range(minPieces, maxPieces + 1);
        foreach (var anchor in PickAnchors(count))
        {
            Instantiate(debrisPrefab, anchor.position, Quaternion.Euler(0f, 0f, 180f));
            yield return new WaitForSeconds(Random.Range(delayBetweenDrops.x, delayBetweenDrops.y));
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
            //bool playerMissing = player == null;

            float distanceToPlayer = Mathf.Abs(anchor.position.x - player.position.x);

            if (distanceToPlayer <= maxDistanceFromPlayer)
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