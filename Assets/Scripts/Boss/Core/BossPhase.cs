using System;
using UnityEngine;

// One entry in the boss's phase list. Element 0 is the starting phase.
// The boss enters phase N once its health fraction drops to or below phases[N].healthThreshold.
[Serializable]
public class BossPhase
{
    public string name = "Phase";
    [Range(0f, 1f)] public float healthThreshold = 1f;
}
