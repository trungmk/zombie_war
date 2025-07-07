using UnityEngine;
using System;
using System.Collections;
using Core;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

public class GrenadeComponent : BaseComponent
{
    [Header("Grenade Settings")]
    [SerializeField]
    private Transform _throwPoint;

    [SerializeField]
    private int _grenadeCount = 3;

    [SerializeField]
    private LayerMask _damageableLayers = -1;

    [SerializeField]
    private string _grenadePoolName = "Grenade";

    [Header("Throw Settings")]
    [SerializeField]
    private float _trajectoryHeight = 2f;

    [SerializeField]
    private bool _useObjectPooling = true;

    private GrenadeWeapon _grenadeWeapon;
    private GrenadeWeaponData _grenadeWeaponData;
    private bool _isAiming = false;

    public event Action<int> OnGrenadeCountChanged;
    public event Action OnGrenadeThrown;

    public int GrenadeCount => _grenadeCount;
    public bool IsAiming => _isAiming;

    private void Start()
    {
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.OnSetupOnGrenadeCompleted += SetGrenadeData;
            WeaponManager.Instance.OnUseGrenade += HandleGrenadeInput;
        }
    }

    private void OnDestroy()
    {
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.OnSetupOnGrenadeCompleted -= SetGrenadeData;
            WeaponManager.Instance.OnUseGrenade -= HandleGrenadeInput;
        }
    }

    public void Setup()
    {
        if (WeaponManager.Instance != null && WeaponManager.Instance.CurrentGrenadeWeapon != null)
        {
            SetGrenadeData(WeaponManager.Instance.CurrentGrenadeWeapon);
        }
    }

    public void SetGrenadeData(GrenadeWeapon grenadeWeapon)
    {
        if (grenadeWeapon == null)
        {
            Debug.LogError("GrenadeWeapon is null");
            return;
        }

        _grenadeWeapon = grenadeWeapon;
        _grenadeWeaponData = grenadeWeapon.GrenadeData;

        if (_grenadeWeapon.gameObject != null)
        {
            _grenadeWeapon.gameObject.SetActive(false);
        }
    }

    public void SetGrenadeCount(int count)
    {
        _grenadeCount = Mathf.Max(0, count);
        OnGrenadeCountChanged?.Invoke(_grenadeCount);
    }

    public bool CanThrowGrenade()
    {
        return _grenadeWeapon != null &&
               _grenadeWeaponData != null &&
               _grenadeCount > 0 &&
               _throwPoint != null;
    }

    public void StartAiming()
    {
        if (CanThrowGrenade())
        {
            _isAiming = true;
        }
    }

    public void StopAiming()
    {
        _isAiming = false;
    }

    public void ThrowGrenade(Vector2 aimDirection)
    {
        if (!CanThrowGrenade())
        {
            Debug.LogWarning("Cannot throw grenade");
            return;
        }

        _isAiming = false;
        _grenadeCount--;
        OnGrenadeCountChanged?.Invoke(_grenadeCount);

        Vector3 throwDirection = CalculateThrowDirection(aimDirection);
        Vector3 spawnPosition = _throwPoint.position;

        GetGrenade(spawnPosition, throwDirection);

        PlayPinPullSound(spawnPosition);
        OnGrenadeThrown?.Invoke();
    }

    private void HandleGrenadeInput(GrenadeWeapon grenadeWeapon)
    {
        Vector2 defaultDirection = Vector2.up;
        ThrowGrenade(defaultDirection);
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

    private void GetGrenade(Vector3 position, Vector3 direction)
    {
        GameObject grenadeObj = _grenadeWeapon.gameObject;

        grenadeObj.SetActive(true);
        SetupGrenadePhysics(grenadeObj, position, direction);
        StartCoroutine(GrenadeFuseCoroutine(grenadeObj, true));
    }

    private void SetupGrenadePhysics(GameObject grenadeObj, Vector3 position, Vector3 direction)
    {
        if (grenadeObj == null) return;

        grenadeObj.transform.position = position;

        Rigidbody rb = grenadeObj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = grenadeObj.AddComponent<Rigidbody>();
        }

        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.mass = 0.5f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;

        Collider collider = grenadeObj.GetComponent<Collider>();
        if (collider == null)
        {
            SphereCollider sphereCollider = grenadeObj.AddComponent<SphereCollider>();
            sphereCollider.radius = 0.1f;
        }

        Vector3 throwForce = direction * _grenadeWeaponData.ThrowForce;
        rb.AddForce(throwForce, ForceMode.VelocityChange);
        rb.AddTorque(UnityEngine.Random.insideUnitSphere * 5f, ForceMode.VelocityChange);
    }

    private void PlayPinPullSound(Vector3 position)
    {
        if (_grenadeWeaponData.PinPullSound != null)
        {
            AudioSource.PlayClipAtPoint(_grenadeWeaponData.PinPullSound, position);
        }
    }

    private IEnumerator GrenadeFuseCoroutine(GameObject grenadeObj, bool returnToPool)
    {
        if (grenadeObj == null || _grenadeWeaponData == null)
        {
            yield break;
        }

        yield return new WaitForSeconds(_grenadeWeaponData.CountDownTime);

        if (grenadeObj == null)
        {
            yield break;
        }

        Vector3 explosionPosition = grenadeObj.transform.position;

        CreateExplosionEffect(explosionPosition);
        PlayExplosionSound(explosionPosition);
        ApplyExplosionDamage(explosionPosition);

        if (returnToPool)
        {
            ObjectPooling.Instance.ReturnToPool(grenadeObj);
        }
        else
        {
            Destroy(grenadeObj);
        }
    }

    private void CreateExplosionEffect(Vector3 position)
    {
        if (_grenadeWeaponData.ExplosionEffect != null)
        {
            GameObject explosion = Instantiate(_grenadeWeaponData.ExplosionEffect, position, Quaternion.identity);

            ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                float duration = ps.main.duration + ps.main.startLifetime.constantMax;
                Destroy(explosion, duration);
            }
            else
            {
                Destroy(explosion, 5f);
            }
        }
    }

    private void PlayExplosionSound(Vector3 position)
    {
        if (_grenadeWeaponData.ExplosionSound != null)
        {
            AudioSource.PlayClipAtPoint(_grenadeWeaponData.ExplosionSound, position);
        }
    }

    private void ApplyExplosionDamage(Vector3 explosionPosition)
    {
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, _grenadeWeaponData.ExplosionRadius, _damageableLayers);

        foreach (Collider hit in colliders)
        {
            if (hit == null) continue;

            float distance = Vector3.Distance(explosionPosition, hit.transform.position);
            float damageMultiplier = 1f - (distance / _grenadeWeaponData.ExplosionRadius);
            damageMultiplier = Mathf.Clamp01(damageMultiplier);

            ITakeDamage damageable = hit.GetComponent<ITakeDamage>();
            if (damageable != null)
            {
                int damage = Mathf.RoundToInt(_grenadeWeaponData.Damage * damageMultiplier);
                damageable.TakeDamage(damage);
            }

            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null && !rb.isKinematic)
            {
                float forceMultiplier = damageMultiplier * 0.5f;
                rb.AddExplosionForce(_grenadeWeaponData.ExplosionForce * forceMultiplier,
                                   explosionPosition,
                                   _grenadeWeaponData.ExplosionRadius);
            }
        }
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

            if (_grenadeWeaponData != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_throwPoint.position, _grenadeWeaponData.ExplosionRadius);
            }
        }
    }
}