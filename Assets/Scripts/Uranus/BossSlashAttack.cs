using System.Collections;
using UnityEngine;

/// <summary>
/// Boss attack: spawns several random slash lines across the arena.
/// Each slash shows a flashing warning line, then after telegraphTime
/// it "fires" and damages the player if they are standing on the line.
/// </summary>
public class BossSlashAttack : MonoBehaviour
{
    [Header("Slash Count")]
    public int minSlashes = 6;
    public int maxSlashes = 8;

    [Header("Timing (seconds)")]
    [Tooltip("How long the warning line shows before the slash hits")]
    public float telegraphTime = 1f;
    [Tooltip("Gap between each slash starting its warning. 0 = all fire together")]
    public float delayBetweenSlashes = 0.12f;
    [Tooltip("How long the slash is dangerous after it fires")]
    public float activeTime = 0.15f;
    [Tooltip("Visual fade-out after the hit window")]
    public float fadeTime = 0.2f;

    [Header("Shape")]
    public float slashLength = 40f;
    public float slashThickness = 1.2f;   // also the hitbox thickness
    public float telegraphWidth = 0.1f;   // thin warning line

    [Header("Placement")]
    public Transform player;
    [Tooltip("Bottom-left corner of the arena in world space")]
    public Vector2 arenaMin = new Vector2(-8f, -4f);
    [Tooltip("Top-right corner of the arena in world space")]
    public Vector2 arenaMax = new Vector2(8f, 4f);
    [Range(0f, 1f)]
    [Tooltip("Chance a slash is placed near the player instead of fully random")]
    public float aimAtPlayerChance = 0.4f;
    public float aimJitter = 2f;

    [Header("Damage")]
    public int damage = 1;
    public LayerMask playerLayer;

    [Header("Visuals")]
    [Tooltip("Optional. Leave empty to use a default sprite material")]
    public Material lineMaterial;
    public Color telegraphColor = new Color(1f, 0.2f, 0.2f, 0.8f);
    public Color slashColor = Color.white;

    int runningSlashes;
    bool attacking;

    /// <summary>True while any slash is still warning/active. Your boss AI can wait on this.</summary>
    public bool IsAttacking => attacking;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            StartAttack();
    }
    /// <summary>Call this from your boss AI / state machine / animation event.</summary>
    public void StartAttack()
    {
        if (!attacking)
            StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        attacking = true;
        int count = Random.Range(minSlashes, maxSlashes + 1);
        runningSlashes = 0;

        for (int i = 0; i < count; i++)
        {
            runningSlashes++;
            StartCoroutine(SlashRoutine());
            if (delayBetweenSlashes > 0f)
                yield return new WaitForSeconds(delayBetweenSlashes);
        }

        // Wait until every slash has finished
        while (runningSlashes > 0)
            yield return null;

        attacking = false;
    }

    IEnumerator SlashRoutine()
    {
        Vector2 center = PickCenter();
        float angle = Random.Range(0f, 180f);
        float rad = angle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        Vector2 a = center - dir * (slashLength * 0.5f);
        Vector2 b = center + dir * (slashLength * 0.5f);
        LineRenderer lr = CreateLine(a, b);

        // 1) Telegraph: flashing thin warning line
        float t = 0f;
        SetWidth(lr, telegraphWidth);
        while (t < telegraphTime)
        {
            t += Time.deltaTime;
            Color c = telegraphColor;
            c.a = Mathf.Lerp(0.25f, telegraphColor.a, Mathf.PingPong(t * 8f, 1f));
            SetColor(lr, c);
            yield return null;
        }

        // 2) Slash fires: wide bright line + damage window
        SetColor(lr, slashColor);
        SetWidth(lr, slashThickness);
        bool hasHit = false;
        t = 0f;
        while (t < activeTime)
        {
            t += Time.deltaTime;
            if (!hasHit)
                hasHit = TryHitPlayer(center, angle);
            yield return null;
        }

        // 3) Fade out (no damage)
        t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float k = 1f - (t / fadeTime);
            Color c = slashColor;
            c.a = k;
            SetColor(lr, c);
            SetWidth(lr, slashThickness * k);
            yield return null;
        }

        Destroy(lr.gameObject);
        runningSlashes--;
    }

    Vector2 PickCenter()
    {
        if (player != null && Random.value < aimAtPlayerChance)
            return (Vector2)player.position + Random.insideUnitCircle * aimJitter;

        return new Vector2(
            Random.Range(arenaMin.x, arenaMax.x),
            Random.Range(arenaMin.y, arenaMax.y));
    }

    bool TryHitPlayer(Vector2 center, float angle)
    {
        // Rotated box along the slash line: x = length, y = thickness
        Collider2D hit = Physics2D.OverlapBox(
            center,
            new Vector2(slashLength, slashThickness),
            angle,
            playerLayer);

        if (hit == null) return false;

        // Replace this with your own player health call, e.g.:
        // hit.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        hit.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        return true;
    }

    LineRenderer CreateLine(Vector2 a, Vector2 b)
    {
        GameObject go = new GameObject("BossSlash");
        go.transform.SetParent(transform, false);

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.SetPosition(0, a);
        lr.SetPosition(1, b);
        lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.sortingOrder = 50;
        lr.numCapVertices = 0;
        return lr;
    }

    static void SetWidth(LineRenderer lr, float w)
    {
        lr.startWidth = w;
        lr.endWidth = w;
    }

    static void SetColor(LineRenderer lr, Color c)
    {
        lr.startColor = c;
        lr.endColor = c;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = (arenaMin + arenaMax) * 0.5f;
        Vector3 size = arenaMax - arenaMin;
        Gizmos.DrawWireCube(center, size);
    }
}