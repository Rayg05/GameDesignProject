using UnityEngine;

public class GrenadeItem : MonoBehaviour
{
    [SerializeField] private float timeToExplode = 5f;
    [SerializeField] private float cooldownDuration = 10f;
    [SerializeField] private int usages = 3;
    [SerializeField] private float grenadeGravity = 2f;       
    [SerializeField] private float minThrowSpeed = 6f;
    [SerializeField] private float maxThrowSpeed = 18f;    
    [SerializeField] private float spinSpeed = 360f;
    [SerializeField] private EquipmentData grenadeItem;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private GameObject grenadePrefab;
    private GameObject activeGrenade;
    private GrenadeAttackArea activeArea;

    private bool isActive = false; 
    private bool onCooldown = false;
    private float timer = 0f;
    private Camera cam;

    public static EquipmentData Data { get; private set; }

    void Awake()
    {
        cam = Camera.main;
        if (grenadeItem != null) Data = grenadeItem;
    }

    //same format as the strengthItem script
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && Loadout.IsEquipped(grenadeItem) && Time.timeScale > 0f)
        {
            if (isActive)
            {
                DetonateEarly();
            }
            else if (!onCooldown && usages > 0)
            {
                ThrowGrenade();
                isActive = true;
                timer = 0f;
                ItemTimers.StartActive(grenadeItem, timeToExplode);
                usages--;
                Debug.Log("Grenade out!");
            }
            else if (onCooldown)
            {
                Debug.Log("still on cooldown!");
            }
        }

        if (isActive)
        {
            timer += Time.deltaTime;
            if (timer >= timeToExplode) EndActive();
        }
        else if (onCooldown)
        {
            timer += Time.deltaTime;
            if (timer >= cooldownDuration)
            {
                onCooldown = false;
                timer = 0f;
                Debug.Log(usages > 0 ? "Cooldown finished, ready to use again" : "Out of Grenades");
            }
        }
    }

    private void EndActive()
    {
        isActive = false;
        onCooldown = true;
        timer = 0f;
        ItemTimers.StartCooldown(grenadeItem, cooldownDuration);
        Debug.Log("Grenade exploded, now on cooldown");
    }
    private void DetonateEarly()
    {
        if (activeArea != null) activeArea.Detonate();
        else if (activeGrenade != null) Destroy(activeGrenade);

        EndActive();
    }
    private void ThrowGrenade()
    {
        //uses same functionality as projectile weapon to get mouse position 
        //for aiming purposes 

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(cam.transform.position.z - firePoint.position.z);
        Vector2 mouseWorld = cam.ScreenToWorldPoint(mousePos);
        Vector2 toMouse = mouseWorld - (Vector2)firePoint.position;
        Vector2 direction = toMouse.normalized;

        //farther cursor = harder throw
        float speed = Mathf.Clamp(toMouse.magnitude, minThrowSpeed, maxThrowSpeed);

        Vector2 launchDir = (direction + Vector2.up).normalized;

        //no aim rotation, the grenade spins instead
        GameObject grenade = Instantiate(grenadePrefab, firePoint.position, Quaternion.identity);

        //don't collide with the player
        Transform shooter = playerRoot != null ? playerRoot : transform.root;
        foreach (Collider2D pc in shooter.GetComponentsInChildren<Collider2D>())
            foreach (Collider2D bc in grenade.GetComponentsInChildren<Collider2D>())
                Physics2D.IgnoreCollision(bc, pc);

        // movement with gravity
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        rb.gravityScale = grenadeGravity;
        rb.linearDamping = 0f;
        rb.linearVelocity = launchDir * speed;
        rb.angularVelocity = -Mathf.Sign(direction.x) * spinSpeed;

        //calls start fuse function so damage can be done 
        activeGrenade = grenade;
        activeArea = grenade.GetComponentInChildren<GrenadeAttackArea>();
        if (activeArea != null) activeArea.StartFuse(timeToExplode);
        else Destroy(grenade, timeToExplode);
    }
}