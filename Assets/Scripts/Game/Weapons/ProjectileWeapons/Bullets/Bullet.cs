using Core;
using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] 
    private float _lifeTime = 3f;

    [SerializeField] 
    private GameObject _hitEffectPrefab;

    private Vector3 _direction;

    private ProjectileWeaponData _currentWeapon;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    public void Initialize(Vector3 direction, ProjectileWeaponData currentWeapon)
    {
        _direction = direction;
        _currentWeapon = currentWeapon;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var damageable = collision.collider.GetComponent<ITakeDamage>();
        if (damageable != null)
        {
            damageable.TakeDamage(_currentWeapon.Damage);
        }

        if (_hitEffectPrefab != null)
        {
            Instantiate(_hitEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}