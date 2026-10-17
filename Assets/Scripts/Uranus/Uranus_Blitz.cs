using UnityEngine;

public class Uranus_Blitz : IState
{
    readonly Boss uranus;
    readonly float rEX;
    readonly float lEX;
    readonly GameObject hitbox;

    float targetX;
    float curY;
    float startTime;

    float uranusSpd = 20f;   // units per second (was 0.1 per frame)
    float duration = 20f;   // total time spent blitzing

    public Uranus_Blitz(Boss boss)
    {
        uranus = boss;
        rEX = uranus.rightEdgeX;
        lEX = uranus.leftEdgeX;

        Transform hb = uranus.transform.Find("Hitbox");
        hitbox = hb != null ? hb.gameObject : null;
        if (hitbox == null)
            Debug.LogWarning("Uranus has no child named 'Hitbox'. Blitz won't deal damage.");
    }

    public void Enter()
    {
        Debug.Log("Entering Uranus_Blitz.");
        startTime = Time.time;
        curY = uranus.transform.position.y;
        if (hitbox != null) hitbox.SetActive(true);

        // Start by dashing to whichever edge is farther away.
        float x = uranus.transform.position.x;
        targetX = Mathf.Abs(x - rEX) > Mathf.Abs(x - lEX) ? rEX : lEX;
    }

    public void Execute()
    {
        // Time's up: stop and move on.
        if (Time.time - startTime >= duration)
        {
            uranus.ChooseNextState(this);
            return;
        }

        uranus.FaceTarget();

        // Move toward the current edge.
        Vector3 pos = uranus.transform.position;
        pos.x = Mathf.MoveTowards(pos.x, targetX, uranusSpd * Time.deltaTime);
        pos.y = curY;
        uranus.transform.position = pos;

        // Arrived: flip to the opposite edge.
        if (Mathf.Approximately(pos.x, targetX))
            targetX = (targetX == rEX) ? lEX : rEX;
    }

    public void Exit()
    {
        if (hitbox != null) hitbox.SetActive(false);
        Debug.Log("Exiting Uranus_Blitz.");
    }
}