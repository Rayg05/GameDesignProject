using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Shared buff/cooldown timers for usable items, so the HUD can show them.
// Items call StartActive/StartCooldown, UI reads the remaining time.
// Several components for the same item (like the StrengthItem per hitbox) can all report, it's the same timer.
public static class ItemTimers
{
    private class Timer
    {
        public float activeEnd, activeLength;
        public float cooldownEnd, cooldownLength;
    }

    private static readonly Dictionary<EquipmentData, Timer> timers = new Dictionary<EquipmentData, Timer>();

    public static void StartActive(EquipmentData item, float duration)
    {
        Timer t = Get(item);
        if (t == null) return;
        t.activeEnd = Time.time + duration;
        t.activeLength = duration;
        t.cooldownEnd = 0f;
    }

    public static void StartCooldown(EquipmentData item, float duration)
    {
        Timer t = Get(item);
        if (t == null) return;
        t.activeEnd = 0f;
        t.cooldownEnd = Time.time + duration;
        t.cooldownLength = duration;
    }

    public static float ActiveRemaining(EquipmentData item) =>
        item != null && timers.TryGetValue(item, out Timer t) ? Mathf.Max(0f, t.activeEnd - Time.time) : 0f;

    public static float CooldownRemaining(EquipmentData item) =>
        item != null && timers.TryGetValue(item, out Timer t) ? Mathf.Max(0f, t.cooldownEnd - Time.time) : 0f;

    // 1 = cooldown just started, 0 = ready
    public static float CooldownFraction(EquipmentData item)
    {
        if (item == null || !timers.TryGetValue(item, out Timer t) || t.cooldownLength <= 0f) return 0f;
        return Mathf.Clamp01((t.cooldownEnd - Time.time) / t.cooldownLength);
    }

    private static Timer Get(EquipmentData item)
    {
        if (item == null) return null;
        if (!timers.TryGetValue(item, out Timer t))
        {
            t = new Timer();
            timers[item] = t;
        }
        return t;
    }

    // items reset when a scene loads, so their timers should too
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        timers.Clear();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) => timers.Clear();
}
