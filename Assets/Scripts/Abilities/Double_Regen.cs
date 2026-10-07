using UnityEngine;

public class DoubleRegen : MonoBehaviour
{
    [SerializeField] private float multiplier = 2f;

    private Health health;

    //awake instead of start so script doesnt need to be enabled
    //to load upon pressing start. Otherwise this wont work unless
    //toggled on and off again
    void Awake()
    {
        health = GetComponent<Health>();
    }

    //used for equipping ability to player
    void OnEnable()
    {
        health.SetRegenMultiplier(multiplier);
    }

    //used when unequipping ability to player
    void OnDisable()
    {
        health.SetRegenMultiplier(1f);
    }
}