using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [SerializeField]
    private ProjectileWeaponData _weaponData;

    [SerializeField]
    private Transform _firePoint;

    [SerializeField]
    private Laser _laserSight;

    [SerializeField]
    private ParticleSystem _muzzleFlash;

    [SerializeField]
    private float _rotationOffset = 30f;

    private float _lastFireTime;
    private bool _isFiring = false;
    private float _autoFireTimer = 0f;
    private Transform _playerTransform;

    public ProjectileWeaponType ProjectileWeaponType => _weaponData?.GunType ?? ProjectileWeaponType.AK47;
    public ProjectileWeaponData WeaponData => _weaponData;
    public bool IsFiring => _isFiring;

    private Quaternion _rotationOffsetQuaternion;

    private void Awake()
    {
        _rotationOffsetQuaternion = Quaternion.Euler(0, _rotationOffset, 0);
        HideLaser();
    }

    private void Update()
    {
        if (_laserSight != null && _laserSight.IsVisible && _firePoint != null)
        {
            Vector3 laserDirection = _rotationOffsetQuaternion * transform.forward;
            _laserSight.UpdateLaser(_firePoint.position, laserDirection);
        }

        if (_isFiring)
        {
            _autoFireTimer += Time.deltaTime;

            if (_autoFireTimer >= _weaponData.FireRate)
            {
                AutoFire();
                _autoFireTimer = 0f;
            }
        }
    }

    public void ShowLaser()
    {
        if (_laserSight != null)
        {
            _laserSight.Show();
        }
    }

    public void HideLaser()
    {
        if (_laserSight != null)
        {
            _laserSight.Hide();
        }
    }

    public void Setup()
    {
        ShowLaser();
    }

    public override bool CanFire()
    {
        return Time.time >= _lastFireTime + _weaponData.FireRate;
    }

    public override void Fire(Vector3 direction)
    {
        if (!CanFire() || _weaponData == null || _firePoint == null)
            return;

        PerformFire(direction);
        _lastFireTime = Time.time;
    }

    private void AutoFire()
    {
        if (_weaponData == null || _firePoint == null)
            return;

        if (_playerTransform == null)
        {
            _playerTransform = transform;
        }

        Vector3 direction = _playerTransform.forward;
        Vector3 offsetDirection = ApplyRotationOffset(direction);
        PerformFire(offsetDirection);
        _lastFireTime = Time.time;
    }

    private void PerformFire(Vector3 direction)
    {
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
    }

    public void StartAutoFiring(Transform playerTransform)
    {
        _isFiring = true;
        _autoFireTimer = 0f;
        _playerTransform = playerTransform != null ? _playerTransform : transform;
        AutoFire();
    }

    public override void StopToFire()
    {
        _isFiring = false;
        _autoFireTimer = 0f;

        if (_muzzleFlash != null)
        {
            _muzzleFlash.Stop();
        }
    }

    public void ToggleAutoFiring()
    {
        if (_isFiring)
        {
            StopToFire();
        }
        else
        {
            StartAutoFiring(_playerTransform);
        }
    }

    private void FireSingle(Vector3 direction)
    {
        Vector3 spreadDirection = direction;
        if (_weaponData.RecoilSpread > 0)
        {
            spreadDirection.x += Random.Range(-_weaponData.RecoilSpread, _weaponData.RecoilSpread) * 0.01f;
            spreadDirection.z += Random.Range(-_weaponData.RecoilSpread, _weaponData.RecoilSpread) * 0.01f;
        }

        CreateBullet(spreadDirection).Forget();

        if (_muzzleFlash != null)
        {
            _muzzleFlash.Play();
        }
    }

    private void FireShotgun(Vector3 direction)
    {
        for (int i = 0; i < _weaponData.PelletsPerShot; i++)
        {
            Vector3 spreadDirection = CalculateSpreadDirection(direction);
            CreateBullet(spreadDirection).Forget();
        }

        if (_muzzleFlash != null)
        {
            _muzzleFlash.Play();
        }
    }

    private Vector3 CalculateSpreadDirection(Vector3 baseDirection)
    {
        float randomAngleX = Random.Range(-_weaponData.SpreadAngle, _weaponData.SpreadAngle);
        float randomAngleY = Random.Range(-_weaponData.SpreadAngle, _weaponData.SpreadAngle);

        Quaternion spreadRotation = Quaternion.Euler(randomAngleX, randomAngleY, 0);
        return spreadRotation * baseDirection;
    }

    private async UniTaskVoid CreateBullet(Vector3 direction)
    {
        Bullet bullet = await ObjectPooling.Instance.Get<Bullet>(_weaponData.BulletName);
        bullet.transform.position = _firePoint.position;
        bullet.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        bullet.Initialize(direction, _weaponData);
    }

    private Vector3 ApplyRotationOffset(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f)
            return direction;

        return _rotationOffsetQuaternion * direction;
    }
}