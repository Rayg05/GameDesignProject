using UnityEngine;

public class Uranus_Blitz : IState
{
    Boss uranus;
    float rEX;
    float lEX;
    float targetX;
    float curY;
    float uranusDir;
    float uranusSpd = 0.1f;
    bool dashing = false;
    GameObject hitbox;
    public Uranus_Blitz(Boss boss)
    {
        uranus = boss;
        rEX = uranus.rightEdgeX;
        lEX = uranus.leftEdgeX;
        curY = uranus.transform.position.y;

        // Look up the hitbox once. It may not exist yet, so don't assume it does.
        Transform hb = uranus.transform.Find("Hitbox");
        hitbox = hb != null ? hb.gameObject : null;
        if (hitbox == null)
            Debug.LogWarning("Uranus has no child named 'Hitbox'. Blitz won't deal damage.");
    }

    public void Enter()
    {
        if (hitbox != null) hitbox.SetActive(true);
        Debug.Log("Entering Uranus_Dash.");
    }

    public void Execute()
    {
        // Face player.
        uranus.FaceTarget();

        // Pick a point.
        if (!dashing)
        {
            uranusDir = uranus.transform.localScale.x;
            // Pick right or left point depending on where uranus is facing.
            if (uranusDir < 0) { targetX = rEX; } else { targetX = lEX; }
            dashing = true;
        }
        else
        {
            // Move uranus.
            uranus.transform.position += new Vector3((-1) * uranusDir * uranusSpd, 0.0f, 0.0f);

            // Have we arrived?
            bool arrived = (uranusDir < 0) ? (uranus.transform.position.x >= rEX) : (uranus.transform.position.x <= lEX);

            // Exit when arrived.
            if (arrived) { uranus.ChooseNextState(this); }
        }
    }

    public void Exit()
    {
        dashing = false;
        if (hitbox != null) hitbox.SetActive(false);
        Debug.Log("Exiting uranus_Dash.");
    }
}