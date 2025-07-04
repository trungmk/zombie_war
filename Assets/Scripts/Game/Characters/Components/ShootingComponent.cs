using UnityEngine;

public class ShootingComponent : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private GunData[] _availableWeapons;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private int _currentWeaponIndex = 0;

    [Header("Effects")]
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private AudioSource _audioSource;

    private float _lastFireTime;
    private Animator _weaponAnimator;
    private GunData _currentWeapon;

    private void Awake()
    {
        _weaponAnimator = GetComponent<Animator>();
        if (_availableWeapons.Length > 0)
            _currentWeapon = _availableWeapons[0];
    }

    public void Shoot(Vector2 aimDirection)
    {
        if (!CanShoot()) return;

        _weaponAnimator?.SetTrigger("Recoil");
        _muzzleFlash?.Play();
        _audioSource?.PlayOneShot(_currentWeapon.ShootSound);
        SpawnBullet(aimDirection);

        _lastFireTime = Time.time;
    }

    private void SpawnBullet(Vector2 aimDirection)
    {
        Vector3 direction = new Vector3(aimDirection.x, 0, aimDirection.y);
        if (direction.sqrMagnitude < 0.01f) direction = transform.forward;

        GameObject bulletGO = Instantiate(_currentWeapon.BulletPrefab, _firePoint.position,
            Quaternion.LookRotation(direction));

        var bullet = bulletGO.GetComponent<Bullet>();
        bullet?.Initialize(direction, _currentWeapon);
    }

    public void SwitchWeapon()
    {
        _currentWeaponIndex = (_currentWeaponIndex + 1) % _availableWeapons.Length;
        _currentWeapon = _availableWeapons[_currentWeaponIndex];

        // Update UI, animation, etc.
        OnWeaponChanged?.Invoke(_currentWeapon);
    }

    public bool CanShoot()
    {
        return Time.time >= _lastFireTime + _currentWeapon.FireRate;
    }

    public System.Action<GunData> OnWeaponChanged;
}