using System;
using System.IO;
using UnityEngine;

public class Projectile_Weapon : MonoBehaviour
{
    [SerializeField] private int numOfShots = 15;
    [SerializeField] private float reloadCooldown = 10f;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    private int currentAmmo;
    private bool isReloading = false;
    private float reloadTimer = 0f;
    private float nextFireTime = 0f;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        currentAmmo = numOfShots;
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer >= reloadCooldown)
            {
                isReloading = false;
                currentAmmo = numOfShots;
                reloadTimer = 0;
            }
        }
        if (Input.GetMouseButtonDown(0) && !isReloading && currentAmmo > 0 && Time.time >= nextFireTime)
        {
            shoot();
            nextFireTime = Time.time + fireRate;
           
        }
        if(Input.GetKeyDown(KeyCode.R) && currentAmmo <= 0)
        {
            isReloading = true;
        }
    }
    private void shoot()
    {
        currentAmmo--;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 direction = ((Vector2)mouseWorld - (Vector2)firePoint.position).normalized;

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        BasicProjectile proj = bullet.GetComponent<BasicProjectile>();
        proj.speed = bulletSpeed;   
        proj.Shoot(direction);
    }
}
