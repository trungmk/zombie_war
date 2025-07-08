using System.Collections;
using UnityEngine;
using Core;
using System;

public class GrenadeWeapon : Weapon
{
    [SerializeField]
    private GrenadeWeaponData _grenadeData;

    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private ParticleSystem _hitEffect;

    public GrenadeWeaponData GrenadeData => _grenadeData;

    public Action<GrenadeWeapon> OnFireCompleted;

    public bool IsUsing { get; set; }

    private Rigidbody _rigidbody;


    private PooledMono _pooledMono;
         
    private void Awake()
    {
        WeaponType = WeaponType.Grenade;
        _rigidbody = GetComponent<Rigidbody>();
        _pooledMono = GetComponent<PooledMono>();
    }

    public override void Fire(Vector3 direction)
    {
        if (_grenadeData == null || !IsUsing)
        {
            return;
        } 
            
        if (_rigidbody != null)
        {
            Vector3 throwDir = direction.normalized * _grenadeData.ThrowForce;
            _rigidbody.AddForce(throwDir, ForceMode.VelocityChange);
        }

        StartCoroutine(GrenadeFuseCoroutine());
    }

    public override void StopToFire()
    {
        
    }

    private IEnumerator GrenadeFuseCoroutine()
    {
        yield return new WaitForSeconds(_grenadeData.CountDownTime);

        if (_hitEffect != null)
        {
            _hitEffect.Play();
        }

        if (_grenadeData.ExplosionSound != null)
        {
            AudioSource.PlayClipAtPoint(_grenadeData.ExplosionSound, transform.position);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, _grenadeData.ExplosionRadius, _layerMask);
        foreach (var hit in colliders)
        {
            var damageable = hit.GetComponent<ITakeDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(_grenadeData.Damage);
            }

            var rb = hit.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(_grenadeData.ExplosionForce, transform.position, _grenadeData.ExplosionRadius);
            }
        }

        if (OnFireCompleted != null)
        {
            OnFireCompleted(this);
        }

        yield return new WaitForSeconds(2);
        _pooledMono.ReturnToPool(); 
    }
}