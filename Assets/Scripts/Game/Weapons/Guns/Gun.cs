using TMPro.EditorUtilities;
using UnityEngine;

public abstract class Gun : Weapon
{
    [SerializeField]
    private GunType _gunType;

    [SerializeField] 
    protected Transform _firePoint;

    [SerializeField] 
    protected GameObject _bulletPrefab;

    [SerializeField] 
    protected float _bulletSpeed = 20f;

    [SerializeField] 
    protected float _fireRate = 0.1f;

    protected float _lastFireTime;

    public GunType GunType => _gunType;

    public override bool CanFire() => Time.time >= _lastFireTime + _fireRate;

    public override void Fire(Vector3 direction)
    {
        if (!CanFire() || _bulletPrefab == null || _firePoint == null)
        {
            return;
        } 
           
        if (direction.sqrMagnitude < 0.01f)
        {
            direction = transform.forward;
        } 

        var bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.LookRotation(direction, Vector3.up));
        var rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * _bulletSpeed;

        _lastFireTime = Time.time;
    }
}