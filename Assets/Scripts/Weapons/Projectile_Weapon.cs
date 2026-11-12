using UnityEngine;

public class Projectile_Weapon : MonoBehaviour
{
    [SerializeField] private int numOfShots = 15;
    [SerializeField] private float reloadCooldown = 10f;
    [SerializeField] private float bulletSpeed = 50f;
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private float bulletLifetime = 2f;
    [SerializeField] private GameObject[] projectilePrefabs;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private EquipmentData projectileWeapon;

    private int currentAmmo;
    private bool isReloading;
    private float reloadTimer;
    private float nextFireTime;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        currentAmmo = numOfShots;
    }

    void Update()
    {
        //make sure its equipped
        if (!Loadout.IsEquipped(projectileWeapon)) return;

        //reload timer until you can shoot again
        if (isReloading)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer >= reloadCooldown)
            {
                isReloading = false;
                currentAmmo = numOfShots;
                reloadTimer = 0f;
            }
        }

        //when holding down left click and you arent reloading and you have ammo and you are past
        //the minimum fire time shoot and update for the next fire time
        if (Input.GetMouseButton(0) && !isReloading && currentAmmo > 0 && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        //reload whenever you are at 0 or less ammo and press R
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo <= 0 && !isReloading)
        {
            isReloading = true;
            reloadTimer = 0f;
            ItemTimers.StartCooldown(projectileWeapon, reloadCooldown);
        }
    }

    private void Shoot()
    {
        if (projectilePrefabs == null || projectilePrefabs.Length == 0) return;
        currentAmmo--;

        // get mouse position 
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(cam.transform.position.z - firePoint.position.z);
        Vector2 mouseWorld = cam.ScreenToWorldPoint(mousePos);
        Vector2 direction = (mouseWorld - (Vector2)firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // instantiate the game object and the set angle it should shoot at
        GameObject prefab = projectilePrefabs[Random.Range(0, projectilePrefabs.Length)];
        GameObject bullet = Instantiate(prefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));

        //loops through the platers root and makes sure each bullet ignores
        //collision with player colliders 
        Transform shooter = playerRoot != null ? playerRoot : transform.root;
        foreach (Collider2D pc in shooter.GetComponentsInChildren<Collider2D>())
            foreach (Collider2D bc in bullet.GetComponentsInChildren<Collider2D>())
                Physics2D.IgnoreCollision(bc, pc);

        //strength buff compensation 
        Projectile_Area area = bullet.GetComponentInChildren<Projectile_Area>();
        if (area != null) area.damage += DamageModifiers.FlatBonus;

        //object movement 
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.linearVelocity = direction * bulletSpeed;

        //destroy object on its timer or impact 
        Destroy(bullet, bulletLifetime);
        bullet.AddComponent<DestroyOnImpact>();
    }
}

//mini helper for object destruction 
public class DestroyOnImpact : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<DestroyOnImpact>() == null) Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.GetComponentInParent<DestroyOnImpact>() == null) Destroy(gameObject);
    }
}