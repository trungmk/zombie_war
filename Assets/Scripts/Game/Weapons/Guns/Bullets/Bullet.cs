using Core;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] 
    private float _lifeTime = 3f;

    [SerializeField] 
    private int _damage = 10;

    [SerializeField] 
    private GameObject _hitEffectPrefab;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var damageable = collision.collider.GetComponent<ITakeDamage>();
        if (damageable != null)
        {
            damageable.TakeDamage(_damage);
        }

        if (_hitEffectPrefab != null)
        {
            Instantiate(_hitEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    public void SetDamage(int damage)
    {
        _damage = damage;
    }
}