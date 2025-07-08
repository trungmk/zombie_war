using UnityEngine;
using System;
using System.Collections;
using Core;

public class GrenadeComponent : BaseComponent
{
    [SerializeField]
    private Transform _throwPoint;

    [SerializeField]
    private int _grenadeCount = 3;

    [SerializeField]
    private LayerMask _damageableLayers = -1;

    [SerializeField]
    private float _trajectoryHeight = 1f;

    private Player _player;
    public event Action OnGrenadeThrown;

    public int GrenadeCount => _grenadeCount;

    public void Setup(Player player)
    {
        _player = player;
    }

    public void SetGrenadeCount(int count)
    {
        _grenadeCount = Mathf.Max(0, count);
    }

    public bool CanThrowGrenade()
    {
        return _grenadeCount > 0 &&
               _throwPoint != null &&
               WeaponManager.Instance != null &&
               WeaponManager.Instance.CurrentGrenadeWeapon != null;
    }

    public void ThrowGrenade(Vector2 aimDirection)
    {
        if (!CanThrowGrenade())
        {
            Debug.LogWarning("Cannot throw grenade");
            return;
        }

        _grenadeCount--;
        WeaponManager.Instance.Handle_EventUseGrenade();

        GrenadeWeapon grenadeWeapon = WeaponManager.Instance.CurrentGrenadeWeapon;
        if (grenadeWeapon != null)
        {
            Vector3 throwDirection = CalculateThrowDirection(aimDirection);
            Vector3 spawnPosition = _throwPoint.position;

            SetupAndThrowGrenade(grenadeWeapon, spawnPosition, throwDirection);
        }

        if (OnGrenadeThrown != null)
        {
            OnGrenadeThrown();
        }
    }

    private void SetupAndThrowGrenade(GrenadeWeapon grenadeWeapon, Vector3 position, Vector3 direction)
    {
        grenadeWeapon.transform.position = position;
        grenadeWeapon.gameObject.SetActive(true);
        grenadeWeapon.IsUsing = true;
        grenadeWeapon.Fire(direction);
    }

    private Vector3 CalculateThrowDirection(Vector2 aimDirection)
    {
        if (aimDirection.sqrMagnitude < 0.01f)
        {
            aimDirection = Vector2.up;
        }

        Vector3 throwDir = new Vector3(aimDirection.x, _trajectoryHeight, aimDirection.y).normalized;
        return throwDir;
    }

    public void AddGrenades(int amount)
    {
        SetGrenadeCount(_grenadeCount + amount);
    }

    private void OnDrawGizmosSelected()
    {
        if (_throwPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_throwPoint.position, 0.2f);
        }
    }
}