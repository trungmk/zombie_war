using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using MEC;
using System.Collections.Generic;
using UnityEngine.Pool;
using Core;
using Cysharp.Threading.Tasks;

public class Enemy : BaseCharacter, ITakeDamage
{
    [SerializeField]
    private EnemyData _enemyData;

    [SerializeField]
    private NavMeshAgent _navMeshAgent;

    [SerializeField]
    private HealthComponent _healthComponent;

    [SerializeField]
    private EnemyAI _enemyAI;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField]
    private AudioSource _audioSource;

    [Header("Waypoints")]
    [SerializeField]
    private Transform[] _waypoints;

    [SerializeField]
    private ParticleSystem _hitFX;

    [SerializeField]
    private DissolveMesh _dissolveMesh;

    public EnemyData EnemyData => _enemyData;
    public NavMeshAgent NavMeshAgent => _navMeshAgent;
    public HealthComponent HealthComponent => _healthComponent;
    public EnemyAI EnemyAI => _enemyAI;
    public Transform[] Waypoints => _waypoints;
    public Transform PlayerTransform { get; private set; }
    public Rigidbody Rigidbody => _rigidbody;
    public Animator Animator => _animator;

    public AudioSource AudioSource => _audioSource;

    public Action<Enemy> OnEnemyDied { get; set; }
    public Action<Enemy> OnEnemyAttack { get; set; }
    public Action<int, int> OnEnemyTakeDamage { get; set; }

    private bool _hasValidNavMesh = false;
    private PooledMono _pooledMono;
    private CapsuleCollider _capsuleCollider;

    private void Awake()
    {
        _pooledMono = GetComponent<PooledMono>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        CheckNavMeshValidity();
    }

    private void OnEnable()
    {
        _animator.applyRootMotion = false;
        _capsuleCollider.enabled = true;
        _dissolveMesh.ResetValues();
        CheckNavMeshValidity();
    }

    private void CheckNavMeshValidity()
    {
        if (_navMeshAgent == null)
        {
            return;
        }

        NavMeshHit hit;
        _hasValidNavMesh = NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas);

        if (!_hasValidNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out hit, 50f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                _hasValidNavMesh = true;       
            }
            else
            {
                _navMeshAgent.enabled = false;
                _hasValidNavMesh = false;
            }
        }

        if (_hasValidNavMesh && !_navMeshAgent.enabled)
        {
            _navMeshAgent.enabled = true;
        }
    }

    public void Initialize()
    {
        if (_enemyData == null)
        {
            Debug.LogError("EnemyData is not assigned!");
            return;
        }

        _healthComponent.Setup(_enemyData.MaxHealth);
        _healthComponent.OnDied += Handle_OnDied;
        _healthComponent.OnHealthChanged += Handle_OnHealthChanged;

        if (_hasValidNavMesh && _navMeshAgent != null && _navMeshAgent.enabled)
        {
            _navMeshAgent.speed = _enemyData.MoveSpeed;
            _navMeshAgent.angularSpeed = _enemyData.RotationSpeed;
            _navMeshAgent.stoppingDistance = _enemyData.WaypointStoppingDistance;
        }

        if (_enemyAI != null)
        {
            _enemyAI.Initialize(this);
        }

        FindPlayer();
    }

    private void Handle_OnHealthChanged(int currentHealth, int maxHealth)
    {
        if (_hitFX != null)
        {
            _hitFX.Play();
        }

        OnEnemyTakeDamage?.Invoke(currentHealth, maxHealth);
    }

    private void Handle_OnDied()
    {
        if (_audioSource != null && _enemyData.DeathSound != null)
        {
            _audioSource.PlayOneShot(_enemyData.DeathSound);
        }

        _animator.applyRootMotion = true;
        _capsuleCollider.enabled = false;

        Animator.SetBool("IsRunning", false);
        Animator.SetTrigger("Die");
        PlayDeathFX().Forget();

        if (_enemyAI != null)
        {
            _enemyAI.enabled = false;
        } 
            
        if (_navMeshAgent != null)
        {
            _navMeshAgent.enabled = false;
        } 
            

        OnEnemyDied?.Invoke(this);
        ReturnToPool().Forget();
    }

    private async UniTaskVoid ReturnToPool()
    {
        if (_dissolveMesh != null)
        {
            _dissolveMesh.StartToDissolve();
        }
        
        await UniTask.WaitForSeconds(5f);

        _pooledMono.ReturnToPool();
    }

    private async UniTaskVoid PlayDeathFX()
    {
        GameObject go = await ObjectPooling.Instance.Get("DeathFX");

        Vector3 pos = transform.position;
        pos.y += 1;
        go.transform.position = pos;

        await UniTask.WaitForSeconds(1f);

        ObjectPooling.Instance.ReturnToPool(go);
    }

    private void FindPlayer()
    {
        Player playerObject = FindFirstObjectByType<Player>();
        if (playerObject != null)
        {
            PlayerTransform = playerObject.transform;
        }
    }

    public void AttackPlayer()
    {
        if (PlayerTransform == null)
            return;

        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
            _animator.SetBool("IsRunning", false);
        }

        if (_audioSource != null && _enemyData.AttackSound != null)
        {
            _audioSource.PlayOneShot(_enemyData.AttackSound);
        }

        OnEnemyAttack?.Invoke(this);
    }

    public bool IsPlayerInRange(float range)
    {
        if (PlayerTransform == null)
            return false;

        return Vector3.Distance(transform.position, PlayerTransform.position) <= range;
    }

    public void SetWaypoints(Transform[] waypoints)
    {
        _waypoints = waypoints;
    }

    public void TakeDamage(int damageAmount)
    {
        Debug.Log("Hit damage:" + damageAmount);
        _healthComponent.TakeDamage(damageAmount);
    }

    public bool HasValidNavMesh()
    {
        return _hasValidNavMesh && _navMeshAgent != null && _navMeshAgent.enabled;
    }

    public bool PlaceOnNavMesh(Vector3 desiredPosition)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(desiredPosition, out hit, 10f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            _hasValidNavMesh = true;

            if (_navMeshAgent != null && !_navMeshAgent.enabled)
            {
                _navMeshAgent.enabled = true;
            }

            return true;
        }

        return false;
    }
}