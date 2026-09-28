using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int health = 10;

    //private int MAX_HEALTH = 100;
    
    public void Damage(int amount)
    {
        health = health - amount;
        if (health - amount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
