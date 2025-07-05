using System;
using UnityEngine;

public class ShootingComponent : BaseComponent
{
    private float _lastFireTime;
    private ProjectileWeaponData _currentWeaponEquipped;
    private ProjectileWeapon _currentWeapon;

    private Transform _firePoint;

    private void Awake()
    {
        WeaponManager.Instance.OnProjectileWeaponChanged += Handle_OnProjectileWeaponChanged;
    }

    public void Setup(Transform firePoint)
    {
        _firePoint = firePoint;
        _currentWeapon = WeaponManager.Instance.CurrentProjectileWeapon;
    }

    private void Handle_OnProjectileWeaponChanged(ProjectileWeapon weapon)
    {
    }

    public void Shoot(Vector2 aimDirection)
    {
        if (!CanShoot())
        {
            return;
        } 

        SpawnBullet(aimDirection);

        _lastFireTime = Time.time;
    }

    private void SpawnBullet(Vector2 aimDirection)
    {
        Vector3 direction = new Vector3(aimDirection.x, 0, aimDirection.y);
        if (direction.sqrMagnitude < 0.01f) direction = transform.forward;

        GameObject bulletGO = Instantiate(_currentWeaponEquipped.BulletPrefab, _firePoint.position,
            Quaternion.LookRotation(direction));

        var bullet = bulletGO.GetComponent<Bullet>();
        bullet?.Initialize(direction, _currentWeaponEquipped);
    }

    public bool CanShoot()
    {
        return Time.time >= _lastFireTime + _currentWeaponEquipped.FireRate;
    }
}