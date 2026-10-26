using UnityEngine;

// How much damage this hitbox/projectile does to the player. Player_Hurtbox reads it on contact.
// Attack states set it from the boss's Inspector values, so you normally edit damage on the boss
// (Larissa_Boss / Uranus_Boss), not here.
public class BossDamage : MonoBehaviour
{
    public float damage = 5f;

    // Gets or adds a BossDamage on target and sets its damage
    public static BossDamage Apply(GameObject target, float amount)
    {
        if (!target.TryGetComponent(out BossDamage bossDamage))
            bossDamage = target.AddComponent<BossDamage>();
        bossDamage.damage = amount;
        return bossDamage;
    }
}
