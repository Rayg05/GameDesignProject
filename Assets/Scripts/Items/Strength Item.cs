using UnityEngine;

public class StrengthItem : MonoBehaviour
{
    [SerializeField] private JabAttackArea jabAttack; 
    [SerializeField] private int strengthBonus = 3;
    [SerializeField] private float buffDuration = 10f;
    [SerializeField] private float cooldownDuration = 10f;
    [SerializeField] private int usages = 3;

    private int orig_Damage;
    private bool isBuffActive = false;
    private bool onCooldown = false;
    private float timer = 0f;

    void Update()
    {
        if (jabAttack == null) return;

        // Activate buff on key press
        if (Input.GetKeyDown(KeyCode.Q) && !isBuffActive && !onCooldown && usages >0)
        {
            orig_Damage = jabAttack.damage;
            jabAttack.damage += strengthBonus;
            isBuffActive = true;
            timer = 0f;
            usages--;
            Debug.Log("UNLIMITED POWERRRR");
        }
        else if (Input.GetKeyDown(KeyCode.Q) && onCooldown)
        {
            Debug.Log("still on cooldown!");
        }

        // Count down the active buff
        if (isBuffActive)
        {
            timer += Time.deltaTime;
            if (timer >= buffDuration)
            {
                jabAttack.damage = orig_Damage;
                isBuffActive = false;
                onCooldown = true;
                timer = 0f;
                Debug.Log("Buff ended, now on cooldown");
            }
        }
        // Count down the cooldown separately
        else if (onCooldown)
        {
            timer += Time.deltaTime;
            if (timer >= cooldownDuration)
            {
                onCooldown = false;
                timer = 0f;
                if (usages > 0)
                {
                    Debug.Log("Cooldown finished, ready to use again");
                }
                else
                {
                    Debug.Log("Out of Strength items");
                }
            }
        }
    }
}