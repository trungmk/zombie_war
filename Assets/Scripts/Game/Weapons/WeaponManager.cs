using UnityEngine;
using System.Collections.Generic;
using System;
using Core;
using Cysharp.Threading.Tasks;

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
    }

    public void Init()
    {
        if (_projectileWeapons.Count > 0)
        {
            EquipProjectileWeapon(_startProjectileWeaponIndex);
        }
        if (_grenadeWeapons.Count > 0)
        {
            EquipGrenadeWeapon(_startGrenadeWeaponIndex).Forget();
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

    public async UniTaskVoid EquipGrenadeWeapon(int index)
    {
        if (_currentGrenadeWeapon != null)
        {
            _currentGrenadeWeapon.gameObject.SetActive(false);
        }

        _currentGrenadeWeaponIndex = index;
        _currentGrenadeWeapon = await ObjectPooling.Instance.Get<GrenadeWeapon>("Grenade_1");
        _currentGrenadeWeapon.gameObject.SetActive(false);

        if (OnSetupOnGrenadeCompleted != null)
        {
            OnSetupOnGrenadeCompleted(_currentGrenadeWeapon);
        }
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
            Debug.LogWarning("No grenades remaining!");
            return;
        }

        LoadGrenade().Forget();
        _remainingGrenadeCount--;
    }

    private async UniTaskVoid LoadGrenade()
    {
        _currentGrenadeWeapon = await ObjectPooling.Instance.Get<GrenadeWeapon>("Grenade_1");
    }
}