using UnityEngine;

// Saturn leaps in an arc until she's directly above the player, hangs for a moment,
// then dives straight down through the floor. She resurfaces somewhere else and
// repeats, for a total of `diveCount` dives, then rises back to the ground.
// Contact damage comes from BossDamage on her colliders, so the dive hurts on its own.
public class Saturn_Dive : IState {
    enum Phase { Jump, Hang, Dive, Underground, Resurface, Grounded }

    readonly Saturn_Boss saturn;

    Phase phase;
    float timer;
    int divesDone;
    float groundY;        // Y she stands at on the floor (captured on Enter)
    Vector3 jumpStart;

    public Saturn_Dive(Saturn_Boss boss) {
        saturn = boss;
    }

    public void Enter() {
        Debug.Log("Entering Saturn_Dive.");
        groundY = saturn.transform.position.y;
        divesDone = 0;
        StartJump();
    }

    public void Execute() {
        timer += Time.deltaTime;
        Transform t = saturn.transform;

        switch (phase) {
            case Phase.Jump: {
                // x moves linearly toward the player's CURRENT x, y eases out to the apex.
                // That combination traces the rising half of a parabola.
                float p = Mathf.Clamp01(timer / saturn.diveJumpDuration);
                float targetX = saturn.target != null ? saturn.target.position.x : jumpStart.x;
                float apexY = groundY + saturn.diveJumpHeight;
                float x = Mathf.Lerp(jumpStart.x, targetX, p);
                float y = Mathf.Lerp(jumpStart.y, apexY, 1f - (1f - p) * (1f - p));
                t.position = new Vector3(x, y, t.position.z);
                saturn.FaceTarget();

                if (p >= 1f) SetPhase(Phase.Hang);
                break;
            }

            case Phase.Hang:
                // Brief pause above the player as a telegraph.
                if (timer >= saturn.diveHangTime) SetPhase(Phase.Dive);
                break;

            case Phase.Dive:
                t.position += Vector3.down * saturn.diveSpeed * Time.deltaTime;
                if (t.position.y <= groundY - saturn.diveUndergroundDepth) {
                    divesDone++;
                    SetPhase(Phase.Underground);
                }
                break;

            case Phase.Underground:
                if (timer >= saturn.diveUndergroundTime) {
                    // Move to a random spot under the arena, then rise up out of the floor there.
                    t.position = new Vector3(PickSurfaceX(), groundY - saturn.diveUndergroundDepth, t.position.z);
                    SetPhase(Phase.Resurface);
                }
                break;

            case Phase.Resurface: {
                // Pop back up onto the floor at the new spot.
                float p = Mathf.Clamp01(timer / saturn.diveResurfaceDuration);
                float y = Mathf.Lerp(groundY - saturn.diveUndergroundDepth, groundY, 1f - (1f - p) * (1f - p));
                t.position = new Vector3(t.position.x, y, t.position.z);
                if (p >= 1f) {
                    saturn.FaceTarget();
                    // More dives left: pause on the ground, then jump. Otherwise the attack is over.
                    if (divesDone < saturn.diveCount) SetPhase(Phase.Grounded);
                    else saturn.ChooseNextState(this);
                }
                break;
            }

            case Phase.Grounded:
                // Short beat standing on the floor before the next jump.
                saturn.FaceTarget();
                if (timer >= saturn.diveGroundedTime) StartJump();
                break;
        }
    }

    public void Exit() {
        // Make sure she's exactly on the floor if the state is interrupted.
        Vector3 pos = saturn.transform.position;
        saturn.transform.position = new Vector3(pos.x, groundY, pos.z);
        Debug.Log("Exiting Saturn_Dive.");
    }

    void StartJump() {
        jumpStart = saturn.transform.position;
        SetPhase(Phase.Jump);
    }

    void SetPhase(Phase next) {
        phase = next;
        timer = 0f;
    }

    // Random X inside the arena, trying to stay away from the player so it feels like "somewhere else".
    float PickSurfaceX() {
        float left = saturn.leftEdgeX;
        float right = saturn.rightEdgeX;
        if (right <= left) { // edges not set in the inspector, so fall back to a range around her
            left = saturn.transform.position.x - 10f;
            right = saturn.transform.position.x + 10f;
        }

        float playerX = saturn.target != null ? saturn.target.position.x : saturn.transform.position.x;
        float x = Random.Range(left, right);
        for (int i = 0; i < 8 && Mathf.Abs(x - playerX) < saturn.diveMinSurfaceDistance; i++) {
            x = Random.Range(left, right);
        }
        return x;
    }
}
