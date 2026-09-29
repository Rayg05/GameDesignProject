using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackStage { None, Windup, Active, Recovery }

// Base class for one boss attack. Each attack is its own component on the boss (or a child).
// An attack runs Windup -> Active -> Recovery, then goes on cooldown.
// Subclasses override the On... hooks to do the actual attack.
public abstract class BossAttack : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private string attackName = "Attack";
    [SerializeField] private string animationTrigger = ""; // Animator trigger fired on windup, blank = none

    [Header("Timing (seconds)")]
    [SerializeField] protected float windupTime = 0.5f;
    [SerializeField] protected float activeTime = 0.2f;
    [SerializeField] protected float recoveryTime = 0.5f;
    [SerializeField] protected float cooldown = 2f;

    [Header("Selection")]
    [SerializeField] private float minRange = 0f;
    [SerializeField] private float maxRange = 3f;
    [SerializeField] private int minPhase = 0;       // attack unlocks at this phase index
    [SerializeField] private float weight = 1f;      // for weighted random selection

    [Header("Damage")]
    [SerializeField] protected int damage = 1;
    [SerializeField] protected Vector2 hitboxOffset = new Vector2(1f, 0f); // flips with boss facing
    [SerializeField] protected Vector2 hitboxSize = Vector2.one;
    [SerializeField] protected LayerMask targetLayers = ~0;

    public string AttackName => attackName;
    public float Weight => weight;
    public bool IsRunning { get; private set; }
    public AttackStage Stage { get; private set; }
    public bool IsReady => Time.time >= lastFinishedTime + cooldown;

    protected BossController boss;

    private float lastFinishedTime = float.NegativeInfinity;
    private Coroutine routine;
    private readonly List<Collider2D> hitBuffer = new List<Collider2D>();
    private readonly HashSet<Health> alreadyHit = new HashSet<Health>();

    // Override to add extra conditions (target is airborne, boss is below 50% hp, etc)
    public virtual bool CanUse(BossController owner)
    {
        if (IsRunning || !IsReady || owner.CurrentPhase < minPhase) return false;

        float distance = owner.DistanceToTarget();
        return distance >= minRange && distance <= maxRange;
    }

    public void Begin(BossController owner)
    {
        if (IsRunning) return;

        boss = owner;
        routine = StartCoroutine(Run());
    }

    // Stops the attack early (boss died, got staggered, state changed...)
    public void Cancel()
    {
        if (!IsRunning) return;

        if (routine != null) StopCoroutine(routine);
        OnCancelled();
        Complete();
    }

    public void ResetCooldown()
    {
        lastFinishedTime = float.NegativeInfinity;
    }

    private IEnumerator Run()
    {
        IsRunning = true;
        alreadyHit.Clear();

        Stage = AttackStage.Windup;
        boss.TriggerAnimation(animationTrigger);
        OnWindup();
        yield return RunStage(windupTime);

        Stage = AttackStage.Active;
        OnActive();
        yield return RunStage(activeTime);

        Stage = AttackStage.Recovery;
        OnRecovery();
        yield return RunStage(recoveryTime);

        Complete();
    }

    private IEnumerator RunStage(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            OnStageUpdate(Stage);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void Complete()
    {
        IsRunning = false;
        Stage = AttackStage.None;
        routine = null;
        lastFinishedTime = Time.time;
        OnEnd();
    }

    // ---- Hooks for subclasses ----
    protected virtual void OnWindup() { }                     // telegraph: flash, sound, charge-up
    protected virtual void OnActive() { }                     // the hit itself: hitbox, projectile, lunge
    protected virtual void OnRecovery() { }                   // punish window for the player
    protected virtual void OnStageUpdate(AttackStage stage) { } // every frame during any stage
    protected virtual void OnCancelled() { }                  // called before OnEnd when interrupted
    protected virtual void OnEnd() { }                        // always called last: clean up hitboxes/effects

    // ---- Helpers ----

    protected Vector2 HitboxCenter
    {
        get
        {
            int facing = boss != null ? boss.FacingDirection : 1;
            return (Vector2)transform.position + new Vector2(hitboxOffset.x * facing, hitboxOffset.y);
        }
    }

    // Damages every Health in the hitbox once per attack. Returns how many were hit.
    protected int DamageInHitbox()
    {
        ContactFilter2D filter = new ContactFilter2D { useTriggers = true };
        filter.SetLayerMask(targetLayers);
        Physics2D.OverlapBox(HitboxCenter, hitboxSize, 0f, filter, hitBuffer);

        int hits = 0;
        foreach (Collider2D col in hitBuffer)
        {
            Health health = col.GetComponentInParent<Health>();
            if (health == null || !alreadyHit.Add(health)) continue;

            health.Damage(damage);
            hits++;
        }
        return hits;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(HitboxCenter, hitboxSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRange);
        if (minRange > 0f)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, minRange);
        }
    }
}
