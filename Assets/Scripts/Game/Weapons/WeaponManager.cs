using UnityEngine;
using System.Collections.Generic;
using System;

public class WeaponManager : MonoSingleton<WeaponManager>
{
    [SerializeField]
    private List<ProjectileWeapon> _projectileWeaponsPrefabs;

    [SerializeField]
    private List<GrenadeWeapon> _grenadeWeaponsPrefabs;

    [SerializeField]
    private int _startProjectileWeaponIndex = 0;

    [SerializeField]
    private int _startGrenadeWeaponIndex = 0;

    [SerializeField]
    private int _maxGrenadeInit = 3;

    private int _currentProjectileWeaponIndex = 0;
    private int _currentGrenadeWeaponIndex = 0;
    private int _remainingGrenadeCount = 5;

    private ProjectileWeapon _currentProjectileWeapon;
    private GrenadeWeapon _currentGrenadeWeapon;

    public ProjectileWeapon CurrentProjectileWeapon => _currentProjectileWeapon;
    public GrenadeWeapon CurrentGrenadeWeapon => _currentGrenadeWeapon;

    public int CurrentProjectileWeaponIndex => _currentProjectileWeaponIndex;

    public int CurrentGrenadeWeaponIndex => _currentGrenadeWeaponIndex;

    public int ProjectileWeaponCount => _projectileWeapons.Count;

    public int GrenadeWeaponCount => _grenadeWeapons.Count;

    private List<ProjectileWeapon> _projectileWeapons = new List<ProjectileWeapon>();

    private List<GrenadeWeapon> _grenadeWeapons = new List<GrenadeWeapon>();

    public Action<ProjectileWeapon> OnProjectileWeaponChanged;

    public Action<GrenadeWeapon> OnSetupOnGrenadeCompleted { get; set; }

    public Action<GrenadeWeapon> OnUseGrenade { get; set; }


    private void Awake()
    {
        _remainingGrenadeCount = _maxGrenadeInit;
        for (int i = 0; i < _projectileWeaponsPrefabs.Count; i++)
        {
            ProjectileWeapon projectileWeapon = Instantiate(_projectileWeaponsPrefabs[i]);
            projectileWeapon.gameObject.SetActive(false);
            projectileWeapon.transform.SetParent(transform, true);

            _projectileWeapons.Add(projectileWeapon);
        }

        for (int i = 0; i < _grenadeWeaponsPrefabs.Count; i++)
        {
            for (int j = 0; j < _maxGrenadeInit; j++)
            {
                GrenadeWeapon grenadeWeapon = Instantiate(_grenadeWeaponsPrefabs[i]);
                grenadeWeapon.gameObject.SetActive(false);
                grenadeWeapon.transform.SetParent(transform, true);
                grenadeWeapon.OnFireCompleted = Handle_GrenadeFireCompleted;

                _grenadeWeapons.Add(grenadeWeapon);
            }
        }
    }

    private void Handle_GrenadeFireCompleted(GrenadeWeapon grenadeWeapon)
    {
        grenadeWeapon.transform.SetParent(transform);
        grenadeWeapon.gameObject.SetActive(false);
    }

    public void Init()
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
        _currentGrenadeWeapon.gameObject.SetActive(false);

        if (OnSetupOnGrenadeCompleted != null)
        {
            OnSetupOnGrenadeCompleted(_currentGrenadeWeapon);
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

    public List<GrenadeWeapon> GetAllGrenadeWeapons()
    {
        return _grenadeWeapons;
    }

    public void Handle_EventSwapWeapon()
    {
        EquipNextProjectileWeapon();
    }

    public void Handle_EventUseGrenade()
    {
        if (_remainingGrenadeCount <= 0)
        {
            return;
        }

        for (int i = 0; i < _grenadeWeapons.Count; i++)
        {
            if (!_grenadeWeapons[i].gameObject.activeSelf  
               && !_grenadeWeapons[i].gameObject.activeInHierarchy)
            {
                _currentGrenadeWeapon = _grenadeWeapons[i];
            }
        }

        _remainingGrenadeCount--;

        Debug.Log($"Using grenade: {_currentGrenadeWeapon.name}, Remaining grenades: {_remainingGrenadeCount}");
        if (OnUseGrenade != null)
        {
            OnUseGrenade(_currentGrenadeWeapon);
        }
    }
}