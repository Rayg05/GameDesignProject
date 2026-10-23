using UnityEngine;

public class SpearArea : MonoBehaviour
{
    [SerializeField] public int damage = 3;
    [SerializeField] public int bonusDamage = 3;
    [SerializeField] public BoxCollider2D tip;

    void OnTriggerEnter2D(Collider2D collision)
    {
        BossHealth bosshealth = collision.GetComponentInParent<BossHealth>();
    }

}
