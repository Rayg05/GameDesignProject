using UnityEngine;

public class JabAttackArea : MonoBehaviour
{
    [SerializeField] public float damage = 3f;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<Health>()!= null)
        {
            Health health = collider.GetComponent<Health>();
            health.Damage(damage);
        }
    }
}
