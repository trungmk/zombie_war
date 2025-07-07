using Core;
using EpicToonFX;
using MEC;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PooledMono
{
    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField]
    private ParticleSystem _hitEffect;

    private Vector3 _direction;
    private ProjectileWeaponData _currentWeapon;
    private float _spawnTime;
    private bool _isInitialized;
    private float _lifeTime;

    private void OnDisable()
    {
        DisableHitFX();
        _isInitialized = false;
    }

    private void OnEnable()
    {
        DisableHitFX();
    }

    public void Initialize(Vector3 direction, ProjectileWeaponData currentWeapon)
    {
        _direction = direction.normalized;
        _currentWeapon = currentWeapon;
        _spawnTime = Time.time;
        _lifeTime = currentWeapon.LifeTime;

        if (_hitEffect != null)
        {
            _hitEffect.gameObject.SetActive(false);
        }

        //if (_direction != Vector3.zero)
        //{
        //    transform.rotation = Quaternion.LookRotation(_direction);
        //}

        _isInitialized = true;
    }

    private void FixedUpdate()
    {
        if (!_isInitialized || _currentWeapon == null)
        {
            return;
        }

        transform.position += _direction * _currentWeapon.BulletSpeed * Time.fixedDeltaTime;
    }

    private void Update()
    {
        if (_isInitialized && Time.time - _spawnTime >= _lifeTime)
        {
            _isInitialized = false;
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isInitialized)
        {
            return;
        }

        Debug.Log($"Bullet hit: {other.name}");

        var damageable = other.GetComponent<ITakeDamage>();
        if (damageable != null && _currentWeapon != null)
        {
            damageable.TakeDamage(_currentWeapon.Damage);
        }

        if (_hitEffect != null)
        {
            _hitEffect.gameObject.SetActive(true);
            _hitEffect.Play();
        }

        _isInitialized = false;
        Timing.RunCoroutine(ReturnToObjectPooling());
    }

    private IEnumerator<float> ReturnToObjectPooling()
    {
        yield return Timing.WaitForSeconds(0.1f);

        
        ReturnToPool();
    }    

    private void DisableHitFX()
    {
        if (_hitEffect != null)
        {
            _hitEffect.gameObject.SetActive(false);
        }
    }    
}