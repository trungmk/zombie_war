using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Fire(Vector3 direction)
    {
        if (bulletPrefab == null || firePoint == null) return;
        var bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        var rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * bulletSpeed;
    }
}
