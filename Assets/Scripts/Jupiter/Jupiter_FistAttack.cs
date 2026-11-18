using UnityEngine;

// Attack 1: Jupiter throws one of his fists, which homes in on the player horizontally (see Jupiter_Fist).
// The state lasts until the fist's lifetime runs out, then goes back to Idle.
public class Jupiter_FistAttack : IState {
    readonly Jupiter_Boss jupiter;
    readonly Animator animator;
    Jupiter_Fist fist;
    float timer;

    public Jupiter_FistAttack(Jupiter_Boss boss) {
        jupiter = boss;
        animator = jupiter.GetComponent<Animator>();
    }

    public void Enter() {
        Debug.Log("Entering Jupiter_FistAttack.");
        timer = 0f;
        jupiter.FaceTarget();

        if (jupiter.fistPrefab == null) {
            Debug.LogWarning("Jupiter_Boss has no fistPrefab assigned; skipping the fist attack.");
            return;
        }

        // Spawn the fist beside Jupiter, on the side facing the player.
        float side = jupiter.target != null ? Mathf.Sign(jupiter.target.position.x - jupiter.transform.position.x) : -1f;
        Vector3 spawn = jupiter.transform.position + new Vector3(side * jupiter.fistSpawnOffset.x, jupiter.fistSpawnOffset.y, 0f);

        GameObject go = Object.Instantiate(jupiter.fistPrefab, spawn, Quaternion.identity);
        fist = go.GetComponent<Jupiter_Fist>();
        if (fist != null) fist.Launch(jupiter.target, jupiter.fistDamage);
    }

    public void Execute() {
        timer += Time.deltaTime;
        // Done when the fist has destroyed itself (or never spawned), with a safety cap.
        if (fist == null || timer >= jupiter.fistAttackMaxDuration) jupiter.ChooseNextState();
    }

    public void Exit() {
        // If the state is cut short, don't leave a fist flying around.
        if (fist != null) Object.Destroy(fist.gameObject);
        fist = null;
        Debug.Log("Exiting Jupiter_FistAttack.");
    }
}
