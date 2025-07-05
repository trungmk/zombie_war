using UnityEngine;
using System.Collections.Generic;

public class WeaponManager : MonoSingleton<WeaponManager>
{
    [Header("Projectile Weapons")]
    [SerializeField]
    private List<ProjectileWeapon> _projectileWeapons = new List<ProjectileWeapon>();

    [Header("Grenade Weapons")]
    [SerializeField]
    private List<GrenadeWeapon> _grenadeWeapons = new List<GrenadeWeapon>();

    [SerializeField]
    private int _startProjectileWeaponIndex = 0;

    [SerializeField]
    private int _startGrenadeWeaponIndex = 0;

    private int _currentProjectileWeaponIndex = 0;
    private int _currentGrenadeWeaponIndex = 0;

    private ProjectileWeapon _currentProjectileWeapon;
    private GrenadeWeapon _currentGrenadeWeapon;

    public ProjectileWeapon CurrentProjectileWeapon => _currentProjectileWeapon;
    public GrenadeWeapon CurrentGrenadeWeapon => _currentGrenadeWeapon;

    public int CurrentProjectileWeaponIndex => _currentProjectileWeaponIndex;

    public int CurrentGrenadeWeaponIndex => _currentGrenadeWeaponIndex;

    public int ProjectileWeaponCount => _projectileWeapons.Count;

    public int GrenadeWeaponCount => _grenadeWeapons.Count;

    public System.Action<ProjectileWeapon> OnProjectileWeaponChanged;

    public System.Action<GrenadeWeapon> OnGrenadeWeaponChanged;

    private void Awake()
    {
        if (_projectileWeapons.Count > 0)
        {
            EquipProjectileWeapon(_startProjectileWeaponIndex);
        }
        if (_grenadeWeapons.Count > 0)
        {
            EquipGrenadeWeapon(_startGrenadeWeaponIndex);
        }
    }

    public void EquipProjectileWeapon(int index)
    {
        if (index < 0 || index >= _projectileWeapons.Count)
        {
            return;
        }

        if (_currentProjectileWeapon != null)
            _currentProjectileWeapon.gameObject.SetActive(false);

        _currentProjectileWeaponIndex = index;
        _currentProjectileWeapon = _projectileWeapons[_currentProjectileWeaponIndex];
        _currentProjectileWeapon.gameObject.SetActive(true);

        OnProjectileWeaponChanged?.Invoke(_currentProjectileWeapon);
    }

    public void EquipNextProjectileWeapon()
    {
        if (_projectileWeapons.Count <= 1)
        {
            return;
        }

        int nextIndex = (_currentProjectileWeaponIndex + 1) % _projectileWeapons.Count;
        EquipProjectileWeapon(nextIndex);
    }

    public void EquipPreviousProjectileWeapon()
    {
        if (_projectileWeapons.Count <= 1)
        {
            return;
        }

        int prevIndex = (_currentProjectileWeaponIndex - 1 + _projectileWeapons.Count) % _projectileWeapons.Count;
        EquipProjectileWeapon(prevIndex);
    }

    public List<ProjectileWeapon> GetAllProjectileWeapons()
    {
        return _projectileWeapons;
    }

    public void EquipGrenadeWeapon(int index)
    {
        if (index < 0 || index >= _grenadeWeapons.Count)
        {
            return;
        }

        if (_currentGrenadeWeapon != null)
        {
            _currentGrenadeWeapon.gameObject.SetActive(false);
        }

        _currentGrenadeWeaponIndex = index;
        _currentGrenadeWeapon = _grenadeWeapons[_currentGrenadeWeaponIndex];
        _currentGrenadeWeapon.gameObject.SetActive(true);

        if (OnGrenadeWeaponChanged != null)
        {
            OnGrenadeWeaponChanged(_currentGrenadeWeapon);
        }
    }

    public void EquipNextGrenadeWeapon()
    {
        if (_grenadeWeapons.Count <= 1)
        {
            return;
        }

        int nextIndex = (_currentGrenadeWeaponIndex + 1) % _grenadeWeapons.Count;
        EquipGrenadeWeapon(nextIndex);
    }

    public void EquipPreviousGrenadeWeapon()
    {
        if (_grenadeWeapons.Count <= 1)
        {
            return;
        } 
            
        int prevIndex = (_currentGrenadeWeaponIndex - 1 + _grenadeWeapons.Count) % _grenadeWeapons.Count;
        EquipGrenadeWeapon(prevIndex);
    }

    public List<GrenadeWeapon> GetAllGrenadeWeapons()
    {
        return _grenadeWeapons;
    }
}