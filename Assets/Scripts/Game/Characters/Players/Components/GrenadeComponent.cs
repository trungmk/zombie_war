using UnityEngine;
using System;
using System.Collections;

public class GrenadeComponent : BaseComponent
{
    private GrenadeWeapon _grenadeWeapon;

    [SerializeField] 
    private Transform throwPoint;

    [SerializeField] 
    private int grenadeCount = 3;

    [SerializeField] 
    private LayerMask damageableLayers = -1;

    public event Action<int> OnGrenadeCountChanged;
    public event Action OnGrenadeThrown;

    public int GrenadeCount => grenadeCount;

    private bool isAiming = false;

    private GrenadeWeaponData _grenadeWeaponData;

    private void Start()
    {
        WeaponManager.Instance.OnSetupOnGrenadeCompleted += SetGrenadeData;
    }

    public void Setup()
    {
        SetGrenadeData(WeaponManager.Instance.CurrentGrenadeWeapon);
    }

    public void SetGrenadeData(GrenadeWeapon grenadeWeapon)
    {
        _grenadeWeapon = grenadeWeapon;
        _grenadeWeaponData = grenadeWeapon.GrenadeData;
        _grenadeWeapon.gameObject.SetActive(false);
    }

    public void SetGrenadeCount(int count)
    {
        grenadeCount = count;
        OnGrenadeCountChanged?.Invoke(grenadeCount);
    }

    public bool CanThrowGrenade()
    {
        return _grenadeWeapon != null && grenadeCount > 0 && throwPoint != null;
    }

    public void ThrowGrenade(Vector2 aimDirection)
    {
        if (!CanThrowGrenade())
        {
            return;
        }

        isAiming = false;

        grenadeCount--;
        OnGrenadeCountChanged?.Invoke(grenadeCount);

        Vector3 throwDir = new Vector3(aimDirection.x, 0.3f, aimDirection.y).normalized;
        Vector3 spawnPos = throwPoint.position;

        GameObject grenadeObj = CreateGrenadeProjectile(spawnPos, throwDir);

        if (_grenadeWeaponData.PinPullSound != null)
        {
            AudioSource.PlayClipAtPoint(_grenadeWeaponData.PinPullSound, spawnPos);
        }

        OnGrenadeThrown?.Invoke();
    }

    private GameObject CreateGrenadeProjectile(Vector3 position, Vector3 direction)
    {
        GameObject grenadeObj;
        if (_grenadeWeapon != null && _grenadeWeaponData.ExplosionEffect != null)
        {
            grenadeObj = Instantiate(_grenadeWeaponData.ExplosionEffect, position, Quaternion.identity);
        }
        else
        {
            grenadeObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            grenadeObj.transform.position = position;
            grenadeObj.transform.localScale = Vector3.one * 0.2f;
        }

        var rb = grenadeObj.GetComponent<Rigidbody>();
        rb = grenadeObj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = grenadeObj.AddComponent<Rigidbody>();
        }
            

        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.mass = 0.5f;

        Vector3 force = direction * _grenadeWeaponData.ThrowForce;
        rb.AddForce(force, ForceMode.VelocityChange);
        rb.AddTorque(UnityEngine.Random.insideUnitSphere * 5f, ForceMode.VelocityChange);

        StartCoroutine(GrenadeFuseCoroutine(grenadeObj));

        return grenadeObj;
    }

    private IEnumerator GrenadeFuseCoroutine(GameObject grenadeObj)
    {
        yield return new WaitForSeconds(_grenadeWeaponData.CountDownTime);

        if (grenadeObj == null)
        {
            yield break;
        }

        if (_grenadeWeaponData.ExplosionEffect != null)
        {
            Instantiate(_grenadeWeaponData.ExplosionEffect, grenadeObj.transform.position, Quaternion.identity);
        }

        if (_grenadeWeaponData.ExplosionSound != null)
        {
            AudioSource.PlayClipAtPoint(_grenadeWeaponData.ExplosionSound, grenadeObj.transform.position);
        }

        Collider[] colliders = Physics.OverlapSphere(grenadeObj.transform.position, _grenadeWeaponData.ExplosionRadius, damageableLayers);
        foreach (var hit in colliders)
        {
            ITakeDamage damageable = hit.GetComponent<ITakeDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(_grenadeWeaponData.Damage);
            }    

            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(_grenadeWeaponData.ExplosionForce, grenadeObj.transform.position, _grenadeWeaponData.ExplosionRadius);
            }
        }

        Destroy(grenadeObj);
    }
}