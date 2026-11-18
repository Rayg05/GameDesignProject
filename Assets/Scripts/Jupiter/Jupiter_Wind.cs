using UnityEngine;

// Attack 2: Jupiter summons wind for windDuration seconds that keeps pushing the player from one side.
// The side is picked at random each time.
// TODO: only allow this once Jupiter is at half HP (not hooked up yet).
public class Jupiter_Wind : IState {
    readonly Jupiter_Boss jupiter;
    readonly Animator animator;
    Basic_Movement2 player;
    float direction;   // +1 pushes the player right, -1 pushes left
    float timer;

    public Jupiter_Wind(Jupiter_Boss boss) {
        jupiter = boss;
        animator = jupiter.GetComponent<Animator>();
    }

    public void Enter() {
        Debug.Log("Entering Jupiter_Wind.");
        timer = 0f;
        jupiter.FaceTarget();

        if (player == null) {
            if (jupiter.target != null) player = jupiter.target.GetComponentInParent<Basic_Movement2>();
            if (player == null) player = Object.FindFirstObjectByType<Basic_Movement2>();
        }

        direction = Random.value < 0.5f ? -1f : 1f;
        Debug.Log("Wind blowing " + (direction > 0 ? "right" : "left") + " for " + jupiter.windDuration + "s.");
    }

    public void Execute() {
        timer += Time.deltaTime;
        if (player != null) player.SetExternalPush(direction * jupiter.windStrength);
        if (timer >= jupiter.windDuration) jupiter.ChooseNextState();
    }

    public void Exit() {
        // Always stop the wind, even if the state is interrupted.
        if (player != null) player.SetExternalPush(0f);
        Debug.Log("Exiting Jupiter_Wind.");
    }
}
