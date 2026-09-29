using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Base class for every boss. Owns the state machine and wires together health, movement,
// attacks and phases. To make a new boss, subclass this (see Template/TemplateBoss.cs)
// and implement ChooseNextState(), which is the boss's attack pattern.
//
// Fight flow: the hub loads the boss's scene -> the fight starts as soon as the scene
// starts -> the boss dies -> after returnDelay seconds, returnSceneName is loaded.
[RequireComponent(typeof(BossHealth))]
public abstract class BossController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;   // auto-finds the player if left empty
    [SerializeField] private Animator animator;  // auto-finds in children if left empty

    [Header("Fight")]
    [SerializeField] private List<BossPhase> phases = new List<BossPhase>();

    [Header("After Defeat")]
    [SerializeField] private string returnSceneName = "HubWorld"; // must be in Build Settings
    [SerializeField] private float returnDelay = 3f;

    [Header("Generic States")]
    [SerializeField] private float idleDuration = 1f;
    [SerializeField] private float chaseTimeout = 5f;

    public BossHealth Health { get; private set; }
    public BossMovement Movement { get; private set; }
    public Animator Animator => animator;
    public Transform Target => target;
    public IReadOnlyList<BossAttack> Attacks => attacks;
    public IReadOnlyList<BossPhase> Phases => phases;
    public BossStateMachine StateMachine { get; private set; }

    public int CurrentPhase { get; private set; }
    public bool IsFightActive { get; private set; }
    public bool IsDead { get; private set; }
    public int FacingDirection => transform.localScale.x >= 0f ? 1 : -1;

    // Generic states every boss gets for free
    public BossIdleState IdleState { get; private set; }
    public BossChaseState ChaseState { get; private set; }
    public BossAttackState AttackState { get; private set; }
    public BossDeadState DeadState { get; private set; }

    public event Action FightStarted;
    public event Action<int> PhaseChanged; // new phase index
    public event Action BossDied;

    private BossAttack[] attacks;
    private readonly List<BossAttack> usableBuffer = new List<BossAttack>();

    // ---- The part each boss must implement ----

    // Called whenever a state finishes. Return the state to go to next. This is the attack pattern.
    public abstract BossState ChooseNextState();

    // ---- Optional overrides ----

    protected virtual void CreateStates() { }                 // create custom states here
    protected virtual BossState GetFirstState() => IdleState;  // state entered when the fight starts
    protected virtual void OnFightStarted() { }                // runs as soon as the scene starts
    protected virtual void OnDamaged(int amount) { }
    protected virtual void OnPhaseChanged(int newPhase) { }
    protected virtual void OnDied() { }

    // ---- Unity lifecycle ----

    protected virtual void Awake()
    {
        Health = GetComponent<BossHealth>();
        // TryGetComponent gives a real null (not Unity's fake null) so states can safely use Movement?.
        Movement = TryGetComponent(out BossMovement movement) ? movement : null;
        if (animator == null) animator = GetComponentInChildren<Animator>();
        attacks = GetComponentsInChildren<BossAttack>();

        if (Movement != null) Movement.Initialize(this);

        StateMachine = new BossStateMachine();
        IdleState = new BossIdleState(this, idleDuration);
        ChaseState = new BossChaseState(this, chaseTimeout);
        AttackState = new BossAttackState(this);
        DeadState = new BossDeadState(this);
        CreateStates();
    }

    protected virtual void OnEnable()
    {
        Health.Damaged += HandleDamaged;
        Health.Died += HandleDied;
    }

    protected virtual void OnDisable()
    {
        Health.Damaged -= HandleDamaged;
        Health.Died -= HandleDied;
    }

    protected virtual void Start()
    {
        if (target == null)
        {
            Basic_Movement player = FindAnyObjectByType<Basic_Movement>();
            if (player != null) target = player.transform;
        }

        StartFight();
    }

    protected virtual void Update()
    {
        if (IsFightActive) StateMachine.Tick(Time.deltaTime);
    }

    protected virtual void FixedUpdate()
    {
        if (IsFightActive) StateMachine.FixedTick();
    }

    // ---- Public API ----

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Jump straight into an attack from anywhere
    public void PerformAttack(BossAttack attack)
    {
        StateMachine.ChangeState(AttackState.With(attack));
    }

    // Fills results with every attack whose CanUse() passes right now
    public void GetUsableAttacks(List<BossAttack> results)
    {
        results.Clear();
        foreach (BossAttack attack in attacks)
        {
            if (attack.isActiveAndEnabled && attack.CanUse(this))
                results.Add(attack);
        }
    }

    public bool HasUsableAttack()
    {
        GetUsableAttacks(usableBuffer);
        return usableBuffer.Count > 0;
    }

    // Default picks a weighted random usable attack, or null if none. Override for fixed sequences.
    public virtual BossAttack SelectAttack()
    {
        GetUsableAttacks(usableBuffer);
        if (usableBuffer.Count == 0) return null;

        float totalWeight = 0f;
        foreach (BossAttack attack in usableBuffer) totalWeight += Mathf.Max(attack.Weight, 0f);
        if (totalWeight <= 0f) return usableBuffer[0];

        float roll = UnityEngine.Random.Range(0f, totalWeight);
        foreach (BossAttack attack in usableBuffer)
        {
            roll -= Mathf.Max(attack.Weight, 0f);
            if (roll <= 0f) return attack;
        }
        return usableBuffer[usableBuffer.Count - 1];
    }

    // Finds an attack component by type, e.g. GetAttack<SlamAttack>()
    public T GetAttack<T>() where T : BossAttack
    {
        foreach (BossAttack attack in attacks)
        {
            if (attack is T match) return match;
        }
        return null;
    }

    // ---- Helpers ----

    public float DistanceToTarget()
    {
        return target != null ? Vector2.Distance(transform.position, target.position) : Mathf.Infinity;
    }

    public Vector2 DirectionToTarget()
    {
        return target != null ? ((Vector2)(target.position - transform.position)).normalized : Vector2.zero;
    }

    public void FaceTarget()
    {
        if (target != null && Movement != null) Movement.FaceTowards(target.position);
    }

    public void TriggerAnimation(string trigger)
    {
        if (animator != null && !string.IsNullOrEmpty(trigger)) animator.SetTrigger(trigger);
    }

    // ---- Internal ----

    private void StartFight()
    {
        if (IsFightActive || IsDead) return;

        IsFightActive = true;
        CurrentPhase = 0;
        OnFightStarted();
        FightStarted?.Invoke();
        StateMachine.ChangeState(GetFirstState());
    }

    private void HandleDamaged(int amount)
    {
        OnDamaged(amount);
        UpdatePhase();
    }

    private void UpdatePhase()
    {
        // Loop so a big hit can skip through multiple phases
        while (CurrentPhase + 1 < phases.Count && Health.Normalized <= phases[CurrentPhase + 1].healthThreshold)
        {
            CurrentPhase++;
            OnPhaseChanged(CurrentPhase);
            PhaseChanged?.Invoke(CurrentPhase);
        }
    }

    private void HandleDied()
    {
        IsDead = true;
        StateMachine.ChangeState(DeadState);
        OnDied();
        BossDied?.Invoke();

        if (!string.IsNullOrEmpty(returnSceneName))
            StartCoroutine(ReturnAfterDelay());
    }

    private IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSeconds(returnDelay);
        SceneManager.LoadScene(returnSceneName);
    }
}
