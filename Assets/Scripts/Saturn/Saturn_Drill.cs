using UnityEngine;

// Saturn leaps in an arc until she's above where the player was when she jumped, hangs for a moment,
// then dives straight down through the floor. She resurfaces somewhere else and
// repeats, for a total of `diveCount` dives, then rises back to the ground.
// While the attack runs, touching her deals diveDamage instead of her normal contact damage.
public class Saturn_Drill : IState {
    enum Phase { Jump, Hang, Dive, Underground, Resurface, Grounded }

    readonly Saturn_Boss saturn;
    readonly Animator animator;
    static readonly int JumpState = Animator.StringToHash("SaturnVertJump");

    Phase phase;
    float timer;
    int divesDone;
    float groundY;        // Y she stands at on the floor (captured on Enter)
    Vector3 jumpStart;
    float jumpTargetX;    // player's X when the jump started (locked for the whole arc)

    public Saturn_Drill(Saturn_Boss boss) {
        saturn = boss;
        animator = saturn.GetComponent<Animator>();
    }

    public void Enter() {
        Debug.Log("Entering Saturn_Drill.");
        groundY = saturn.transform.position.y;
        divesDone = 0;
        BossDamage.Apply(saturn.gameObject, saturn.diveDamage);
        animator.SetBool("Drill", true);
        StartJump();
    }

    public void Execute() {
        timer += Time.deltaTime;
        Transform t = saturn.transform;

        switch (phase) {
            case Phase.Jump: {
                // x moves linearly toward where the player was when the jump began, y eases out
                // to the apex. That traces the rising half of a parabola along a fixed path:
                // she commits to the spot and doesn't track the player mid-air.
                float p = Mathf.Clamp01(timer / saturn.diveJumpDuration);
                float apexY = groundY + saturn.diveJumpHeight;
                float x = Mathf.Lerp(jumpStart.x, jumpTargetX, p);
                float y = Mathf.Lerp(jumpStart.y, apexY, 1f - (1f - p) * (1f - p));
                t.position = new Vector3(x, y, t.position.z);

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
        animator.SetBool("Drill", false);
        animator.SetBool("DrillDive", false);
        animator.speed = 1f;
        // Make sure she's exactly on the floor if the state is interrupted.
        Vector3 pos = saturn.transform.position;
        saturn.transform.position = new Vector3(pos.x, groundY, pos.z);
        BossDamage.Apply(saturn.gameObject, saturn.contactDamage);
        Debug.Log("Exiting Saturn_Drill.");
    }

    // Jump / Hang: SaturnVertJump plays once (0.8s at 10fps, matching diveJumpDuration) and holds
    //   its last frame (hair spread) through the hang.
    // Dive / Underground / Resurface: SaturnDrill (DrillDive = true).
    // Grounded: frozen on the first jump frame, so the jump plays from the start when she takes off.
    void UpdateAnimation() {
        bool drilling = phase == Phase.Dive || phase == Phase.Underground || phase == Phase.Resurface;
        animator.SetBool("DrillDive", drilling);

        if (phase == Phase.Jump || phase == Phase.Grounded) {
            animator.Play(JumpState, 0, 0f);
            animator.speed = phase == Phase.Grounded ? 0f : 1f;
        } else {
            animator.speed = 1f;
        }
    }

    void StartJump() {
        jumpStart = saturn.transform.position;
        jumpTargetX = saturn.target != null ? saturn.target.position.x : jumpStart.x;
        saturn.FaceTarget(); // face the landing spot once; don't flip mid-air
        SetPhase(Phase.Jump);
    }

    void SetPhase(Phase next) {
        phase = next;
        timer = 0f;
        UpdateAnimation();
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
