using UnityEngine;

public class ShootingComponent : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletSpeed = 20f;
    [SerializeField] private float _fireRate = 0.15f;

    private float _lastFireTime;

    public void Shoot(Vector2 aimDirection)
    {
        Shoot(new Vector3(aimDirection.x, 0, aimDirection.y));
    }

    public void Shoot(Vector3 aimDirection)
    {
        if (Time.time < _lastFireTime + _fireRate)
        {
            return;
        }

        if (_bulletPrefab == null || _firePoint == null)
        {
            return;
        }

        if (aimDirection.sqrMagnitude < 0.01f) aimDirection = transform.forward;

        GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.LookRotation(aimDirection, Vector3.up));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = aimDirection.normalized * _bulletSpeed;

        _lastFireTime = Time.time;
    }

    public void AutoShoot(Vector2 aimDirection)
    {
        if (aimDirection.sqrMagnitude > 0.01f)
        {
            Shoot(aimDirection);
        }
    }

    public bool CanShoot()
    {
        return Time.time >= _lastFireTime + _fireRate;
    }
}