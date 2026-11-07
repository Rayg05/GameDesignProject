using UnityEngine;
using UnityEngine.LowLevel;

public class GrenadeItem : MonoBehaviour
{
    [SerializeField] private GrenadeAttackArea grenadeAttack;
    [SerializeField] private int strengthBonus = 3;
    [SerializeField] private float timeToExplode = 5f;
    [SerializeField] private float cooldownDuration = 10f;
    [SerializeField] private int usages = 3;
    [SerializeField] private float grenadeSpeed = 50f;
    [SerializeField] private EquipmentData grenadeItem;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private GameObject grenadePrefab;

    private int orig_Damage;
    private bool isActive = false;
    private bool onCooldown = false;
    private float timer = 0f;
    private Camera cam;


    // which EquipmentData is the strength item, so the HUD can show its timer
    public static EquipmentData Data { get; private set; }

    void Awake()
    {
        cam = Camera.main;
        if (grenadeItem != null) Data = grenadeItem;
    }

    void Update()
    {
        if (grenadeAttack == null) return;

        // Activate buff on key press
        if (Input.GetKeyDown(KeyCode.Q) && Loadout.IsEquipped(grenadeItem) && Time.timeScale > 0f)
        {
            if (!isActive && !onCooldown && usages > 0)
            {
                Explode();
                orig_Damage = grenadeAttack.damage;
                grenadeAttack.damage += strengthBonus;
                isActive = true;
                timer = 0f;
                ItemTimers.StartActive(grenadeItem, timeToExplode);
                usages--;
                
            }
            else if (onCooldown)
            {
                Debug.Log("still on cooldown!");
            }
        }

        // Count down the active buff
        if (isActive)
        {
            timer += Time.deltaTime;
            if (timer >= cooldownDuration)
            {
                grenadeAttack.damage = orig_Damage;
                isActive = false;
                onCooldown = true;
                ItemTimers.StartCooldown(grenadeItem, cooldownDuration);
                timer = 0f;
                Debug.Log("now on cooldown");
            }
        }
    }
    private void Explode()
    {
        // get mouse position 
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(cam.transform.position.z - firePoint.position.z);
        Vector2 mouseWorld = cam.ScreenToWorldPoint(mousePos);
        Vector2 direction = (mouseWorld - (Vector2)firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject grenade = Instantiate(grenadePrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));

        Transform shooter = playerRoot != null ? playerRoot : transform.root;
        foreach (Collider2D pc in shooter.GetComponentsInChildren<Collider2D>())
            foreach (Collider2D bc in grenade.GetComponentsInChildren<Collider2D>())
                Physics2D.IgnoreCollision(bc, pc);

        //strength buff compensation 
        GrenadeAttackArea area = grenade.GetComponentInChildren<GrenadeAttackArea>();
        if (area != null) area.damage += DamageModifiers.FlatBonus;

        //object movement 
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.linearVelocity = direction * grenadeSpeed;

        //destroy object on its timer or impact 
        Destroy(grenade, timeToExplode);
        
    }
}
