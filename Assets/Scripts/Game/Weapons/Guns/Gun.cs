using UnityEngine;

public class Gun : Weapon
{
    [SerializeField] 
    private GunData _gunData;

    [SerializeField] 
    private Transform _firePoint;

    [SerializeField] 
    private bool _isAutoFiring = false;

    private float _lastFireTime;

    private float _nextAutoFireTime;

    public GunType GunType => _gunData?.GunType ?? GunType.AK47;

    public GunData GunData => _gunData;

    private void Start()
    {
        if (_gunData == null)
        {
            Debug.LogError($"GunData chưa được gán cho {gameObject.name}");
        }
    }

    public override bool CanFire() => Time.time >= _lastFireTime + _gunData.FireRate;

    public override void Fire(Vector3 direction)
    {
        if (!CanFire() || _gunData == null || _firePoint == null)
        {
            return;
        }

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = transform.forward;
        }

        switch (_gunData.GunType)
        {
            case GunType.AK47:
                FireAK47(direction);
                break;

            case GunType.Shotgun:
                FireShotgun(direction);
                break;
        }

        _lastFireTime = Time.time;
    }

    private void FireAK47(Vector3 direction)
    {
        Vector3 spreadDirection = direction;
        if (_gunData.RecoilSpread > 0)
        {
            spreadDirection.x += Random.Range(-_gunData.RecoilSpread, _gunData.RecoilSpread) * 0.01f;
            spreadDirection.z += Random.Range(-_gunData.RecoilSpread, _gunData.RecoilSpread) * 0.01f;
        }

        CreateBullet(spreadDirection);
    }

    private void FireShotgun(Vector3 direction)
    {
        for (int i = 0; i < _gunData.PelletsPerShot; i++)
        {
            Vector3 spreadDirection = CalculateSpreadDirection(direction);
            CreateBullet(spreadDirection);
        }
    }

    private Vector3 CalculateSpreadDirection(Vector3 baseDirection)
    {
        float randomAngleX = Random.Range(-_gunData.SpreadAngle, _gunData.SpreadAngle);
        float randomAngleY = Random.Range(-_gunData.SpreadAngle, _gunData.SpreadAngle);

        Quaternion spreadRotation = Quaternion.Euler(randomAngleX, randomAngleY, 0);
        return spreadRotation * baseDirection;
    }

    private void CreateBullet(Vector3 direction)
    {
        var bullet = Instantiate(_gunData.BulletPrefab, _firePoint.position,
            Quaternion.LookRotation(direction, Vector3.up));

        var rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * _gunData.BulletSpeed;
    }

    public void StartAutoFire(Vector3 direction)
    {
        if (_gunData.IsAutomatic && !_isAutoFiring)
        {
            _isAutoFiring = true;
            InvokeRepeating(nameof(AutoFire), 0f, _gunData.FireRate);
        }
    }

    public void StopAutoFire()
    {
        _isAutoFiring = false;
        CancelInvoke(nameof(AutoFire));
    }

    private void AutoFire()
    {
        if (_isAutoFiring)
        {
            Fire(transform.forward);
        }
    }

    private void OnDisable()
    {
        StopAutoFire();
    }
}