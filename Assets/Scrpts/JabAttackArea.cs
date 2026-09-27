using UnityEngine;

public class JabAttackArea : MonoBehaviour
{
    private int damage = 3;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Trigger entered with: " + collider.gameObject.name);
        if (collider.GetComponent<Health>()!= null)
        {
            Debug.Log("Found Health component, applying damage");
            Health health = collider.GetComponent<Health>();
            health.Damage(damage);
        }
    }
}
