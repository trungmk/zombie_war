using System;
using UnityEngine;

public class ShootingComponent : BaseComponent
{
    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField]
    private float _directionOffset = 50f;

    private ProjectileWeaponData _currentWeaponEquipped;
    private ProjectileWeapon _currentWeapon;
    private Player _player;
    private Transform _weaponHoldPoint;
    private float _rotationSpeed = 360f;

    private void Start()
    {
        WeaponManager.Instance.OnProjectileWeaponChanged += Handle_OnProjectileWeaponChanged;
    }

    private void OnDestroy()
    {
        WeaponManager.Instance.OnProjectileWeaponChanged -= Handle_OnProjectileWeaponChanged;
    }

    public void Setup(Player player)
    {
        _player = player;
        _weaponHoldPoint = _player.WeaponTransform;
        _rotationSpeed = player.PlayerData.RotationSpeed;
        SetCurrentProjectileWeapon(WeaponManager.Instance.CurrentProjectileWeapon);
    }

    private void Handle_OnProjectileWeaponChanged(ProjectileWeapon weapon)
    {
        SetCurrentProjectileWeapon(weapon);
        _player.AnimationController.TriggerChangeWeapon();
    }

    private void SetCurrentProjectileWeapon(ProjectileWeapon projectileWeapon)
    {
        _currentWeapon = projectileWeapon;
        _currentWeapon.transform.SetParent(_weaponHoldPoint, false);
        _currentWeapon.transform.localPosition = Vector3.zero;
        _currentWeapon.transform.localRotation = Quaternion.identity;

        _currentWeapon.Setup(_player.transform);
        _currentWeaponEquipped = _currentWeapon.WeaponData;
    }

    public void AutoShoot(Vector3 aimDirection)
    {
        _player.Movement.Rotate(aimDirection);

        if (!IsAutoShooting())
        {
            _currentWeapon.StartAutoFiring(_player.transform);
        }
    }

    public void StartAutoShooting()
    {
        if (_currentWeapon != null && !_currentWeapon.IsFiring)
        {
            Debug.Log("Starting auto shooting");
            _currentWeapon.StartAutoFiring(_player.transform);
        }
    }

    public void StopShooting()
    {
        if (_currentWeapon != null)
        {
            Debug.Log("Stopping auto shooting");
            _currentWeapon.StopToFire();
        }
    }

    public bool IsAutoShooting()
    {
        return _currentWeapon != null && _currentWeapon.IsFiring;
    }

    public bool CanShoot()
    {
        return _currentWeapon != null && _currentWeapon.CanFire();
    }
}