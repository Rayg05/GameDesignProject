using UnityEngine;

public class StrengthItem : MonoBehaviour
{
    [SerializeField] private int strengthBonus = 3;
    [SerializeField] private float buffDuration = 10f;
    [SerializeField] private float cooldownDuration = 10f;
    [SerializeField] private int usages = 3;
    [SerializeField] private EquipmentData strengthItem;

    private bool isBuffActive = false;
    private bool onCooldown = false;
    private float timer = 0f;

    public static EquipmentData Data { get; private set; }

    void Awake()
    {
        if (strengthItem != null) Data = strengthItem;
    }

    void OnDisable()
    {
        // Don't leave the bonus stuck on if this object is disabled/destroyed mid-buff
        if (isBuffActive) EndBuff(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && Loadout.IsEquipped(strengthItem) && Time.timeScale > 0f)
        {
            if (!isBuffActive && !onCooldown && usages > 0)
            {
                DamageModifiers.FlatBonus += strengthBonus;
                isBuffActive = true;
                timer = 0f;
                ItemTimers.StartActive(strengthItem, buffDuration);
                usages--;
                Debug.Log("UNLIMITED POWERRRR");
            }
            else if (onCooldown)
            {
                Debug.Log("still on cooldown!");
            }
        }

        if (isBuffActive)
        {
            timer += Time.deltaTime;
            if (timer >= buffDuration) EndBuff(true);
        }
        else if (onCooldown)
        {
            timer += Time.deltaTime;
            if (timer >= cooldownDuration)
            {
                onCooldown = false;
                timer = 0f;
                Debug.Log(usages > 0 ? "Cooldown finished, ready to use again" : "Out of Strength items");
            }
        }
    }

    private void EndBuff(bool startCooldown)
    {
        DamageModifiers.FlatBonus -= strengthBonus;
        isBuffActive = false;
        timer = 0f;
        if (startCooldown)
        {
            onCooldown = true;
            ItemTimers.StartCooldown(strengthItem, cooldownDuration);
            Debug.Log("Buff ended, now on cooldown");
        }
    }
}