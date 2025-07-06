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

    public EnemyData EnemyData => _enemyData;
    public NavMeshAgent NavMeshAgent => _navMeshAgent;
    public HealthComponent HealthComponent => _healthComponent;
    public EnemyAI EnemyAI => _enemyAI;
    public Transform[] Waypoints => _waypoints;
    public Transform PlayerTransform { get; private set; }
    public Rigidbody Rigidbody => _rigidbody;
    public Animator Animator => _animator; 

    public Action<Enemy> OnEnemyDied;

    public Action<Enemy> OnEnemyAttack;

    public Action<int, int> OnEnemyTakeDamage;

    private void Start()
    {
        Timing.RunCoroutine(StartAI());
    }

    private void OnEnable()
    {
        _animator.applyRootMotion = false;
    }

    private IEnumerator<float> StartAI()
    {
        yield return Timing.WaitForSeconds(2);

        Initialize();
    }    

    private void Initialize()
    {
        if (_enemyData == null)
        {
            Debug.LogError("EnemyData is not assigned!");
            return;
        }

        _healthComponent.Setup(_enemyData.MaxHealth);
        _healthComponent.OnDied += Handle_OnDied;
        _healthComponent.OnHealthChanged += Handle_OnHealthChanged;

        _navMeshAgent.speed = _enemyData.MoveSpeed;
        _navMeshAgent.angularSpeed = _enemyData.RotationSpeed;
        _navMeshAgent.stoppingDistance = _enemyData.WaypointStoppingDistance;

        _enemyAI.Initialize(this);

        FindPlayer();
    }

    private void Handle_OnHealthChanged(int currentHealth, int maxHealth)
    {
        if (_hitFX != null)
        {
            _hitFX.Play();
        }

        if (OnEnemyTakeDamage != null)
        {
            OnEnemyTakeDamage(currentHealth, maxHealth);
        }
    }

    private void Handle_OnDied()
    {
        if (_audioSource != null && _enemyData.DeathSound != null)
        {
            _audioSource.PlayOneShot(_enemyData.DeathSound);
        }

        _animator.applyRootMotion = true;

        Animator.SetBool("IsRunning", false);
        Animator.SetTrigger("Die");

        // Play death FX
        PlayDeathFX().Forget();

        if (_enemyAI != null) _enemyAI.enabled = false;
        if (_navMeshAgent != null) _navMeshAgent.enabled = false;

        OnEnemyDied?.Invoke(this);
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

        float distanceToPlayer = Vector3.Distance(transform.position, PlayerTransform.position);
        if (distanceToPlayer <= _enemyData.AttackRange)
        {
            var playerHealth = PlayerTransform.GetComponent<ITakeDamage>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_enemyData.AttackDamage);
            }
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
}