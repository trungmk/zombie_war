using UnityEngine;

public class EnemyAttackNode : LeafNode
{
    private Enemy _enemy;
    private Transform _currentTarget;
    private float _lastAttackTime = -999f;
    private bool _isInAttackSequence = false;
    private float _attackSequenceTimer = 0f;
    private const float ATTACK_SEQUENCE_DURATION = 1.0f;

    public EnemyAttackNode(Enemy enemy, Transform currentTarget) : base(null)
    {
        _enemy = enemy;
        _currentTarget = currentTarget;
        SetFunction(AttackPlayer);
    }

    public void SetTarget(Transform target)
    {
        _currentTarget = target;
    }

    private NodeState AttackPlayer()
    {
        if (_enemy == null || _enemy.HealthComponent == null || _enemy.HealthComponent.IsDead)
        {
            ResetAttackState();
            return NodeState.Failure;
        }

        if (_enemy.NavMeshAgent == null || !_enemy.NavMeshAgent.enabled)
        {
            ResetAttackState();
            return NodeState.Failure;
        }

        if (_currentTarget == null)
        {
            ResetAttackState();
            return NodeState.Failure;
        }

        float distanceToPlayer = Vector3.Distance(_enemy.transform.position, _currentTarget.position);

        if (_isInAttackSequence)
        {
            _attackSequenceTimer += Time.deltaTime;
            ForceStopMovement();

            if (_attackSequenceTimer >= ATTACK_SEQUENCE_DURATION)
            {
                CheckFinalPositionAndDealDamage();
                ResetAttackState();
                return NodeState.Success;
            }

            return NodeState.Running;
        }

        if (distanceToPlayer > _enemy.EnemyData.AttackRange)
        {
            return NodeState.Failure;
        }

        if (Time.time - _lastAttackTime < _enemy.EnemyData.AttackCooldown)
        {
            return NodeState.Running;
        }

        StartAttackSequence();
        return NodeState.Running;
    }

    private void StartAttackSequence()
    {
        _isInAttackSequence = true;
        _attackSequenceTimer = 0f;
        _lastAttackTime = Time.time;

        ForceStopMovement();
        RotateTowardsTarget();

        if (_enemy.Animator != null && _enemy.Animator.enabled)
        {
            _enemy.Animator.SetTrigger("Attack");
        }

        if (_enemy.AudioSource != null && _enemy.EnemyData.AttackSound != null)
        {
            _enemy.AudioSource.PlayOneShot(_enemy.EnemyData.AttackSound);
        }

        _enemy.OnEnemyAttack?.Invoke(_enemy);
    }

    private void ForceStopMovement()
    {
        if (_enemy.NavMeshAgent != null && _enemy.NavMeshAgent.enabled)
        {
            
            _enemy.NavMeshAgent.SetDestination(_enemy.transform.position);
            _enemy.NavMeshAgent.isStopped = true;
            _enemy.NavMeshAgent.velocity = Vector3.zero;
            _enemy.NavMeshAgent.updatePosition = false;
            _enemy.NavMeshAgent.updateRotation = false;
        }

        if (_enemy.Animator != null && _enemy.Animator.enabled)
        {
            _enemy.Animator.SetBool("IsRunning", false);
        }
    }

    private void CheckFinalPositionAndDealDamage()
    {
        if (_currentTarget == null)
        {
            return;
        }

        float finalDistance = Vector3.Distance(_enemy.transform.position, _currentTarget.position);

        if (finalDistance <= _enemy.EnemyData.AttackRange)
        {
            var playerHealth = _currentTarget.GetComponent<ITakeDamage>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_enemy.EnemyData.AttackDamage);
            }
        }
    }

    private void RotateTowardsTarget()
    {
        if (_currentTarget == null) return;

        Vector3 direction = (_currentTarget.position - _enemy.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _enemy.transform.rotation = Quaternion.Lerp(_enemy.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void ResetAttackState()
    {
        _isInAttackSequence = false;
        _attackSequenceTimer = 0f;

        if (_enemy.NavMeshAgent != null && _enemy.NavMeshAgent.enabled)
        {
            _enemy.NavMeshAgent.isStopped = false;
        }
    }

    public bool IsAttacking()
    {
        return _isInAttackSequence;
    }
}