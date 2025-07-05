using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [SerializeField]
    private ProjectileWeaponData _weaponData;

    [SerializeField]
    private Transform _firePoint;

    [SerializeField]
    private Laser _laserSight;

    private float _lastFireTime;

    public ProjectileWeaponType GunType => _weaponData?.GunType ?? ProjectileWeaponType.AK47;
    public ProjectileWeaponData WeaponData => _weaponData;

    public override bool CanFire() => Time.time >= _lastFireTime + _weaponData.FireRate;

    private void Start()
    {
        _laserSight.UpdateLaser(_firePoint.position, transform.forward);
    }

    public override void Fire(Vector3 direction)
    {
        if (!CanFire() || _weaponData == null || _firePoint == null)
            return;

        if (direction.sqrMagnitude < 0.01f)
            direction = transform.forward;

        switch (_weaponData.GunType)
        {
            case ProjectileWeaponType.AK47:
                FireSingle(direction);
                break;
            case ProjectileWeaponType.Shotgun:
                FireShotgun(direction);
                break;
            default:
                FireSingle(direction);
                break;
        }

        _lastFireTime = Time.time;
    }

    private void FireSingle(Vector3 direction)
    {
        Vector3 spreadDirection = direction;
        if (_weaponData.RecoilSpread > 0)
        {
            spreadDirection.x += Random.Range(-_weaponData.RecoilSpread, _weaponData.RecoilSpread) * 0.01f;
            spreadDirection.z += Random.Range(-_weaponData.RecoilSpread, _weaponData.RecoilSpread) * 0.01f;
        }

        CreateBullet(spreadDirection);
    }

    private void FireShotgun(Vector3 direction)
    {
        for (int i = 0; i < _weaponData.PelletsPerShot; i++)
        {
            Vector3 spreadDirection = CalculateSpreadDirection(direction);
            CreateBullet(spreadDirection);
        }
    }

    private Vector3 CalculateSpreadDirection(Vector3 baseDirection)
    {
        float randomAngleX = Random.Range(-_weaponData.SpreadAngle, _weaponData.SpreadAngle);
        float randomAngleY = Random.Range(-_weaponData.SpreadAngle, _weaponData.SpreadAngle);

        Quaternion spreadRotation = Quaternion.Euler(randomAngleX, randomAngleY, 0);
        return spreadRotation * baseDirection;
    }

    private void CreateBullet(Vector3 direction)
    {
        var bullet = Instantiate(_weaponData.BulletPrefab, _firePoint.position,
            Quaternion.LookRotation(direction, Vector3.up));

        var rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * _weaponData.BulletSpeed;
    }
}