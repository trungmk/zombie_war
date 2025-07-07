using UnityEngine;

public class EnemyAttackNode : LeafNode
{
    private Enemy _enemy;
    private Transform _currentTarget;
    private float _lastAttackTime = -999f;
    private bool _isInAttackSequence = false;
    private float _attackSequenceTimer = 0f;
    private const float ATTACK_SEQUENCE_DURATION = 1.5f;

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
        // Early exit checks
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

        if (distanceToPlayer > _enemy.EnemyData.AttackRange && !_isInAttackSequence)
        {
            ResetAttackState();
            return NodeState.Failure;
        }

        if (_isInAttackSequence)
        {
            _attackSequenceTimer += Time.deltaTime;
            if (_attackSequenceTimer >= ATTACK_SEQUENCE_DURATION)
            {
                ResetAttackState();
                return NodeState.Success; 
            }

            return NodeState.Running; 
        }

        if (distanceToPlayer <= _enemy.EnemyData.AttackRange &&
            Time.time - _lastAttackTime >= _enemy.EnemyData.AttackCooldown)
        {
            StartAttackSequence();
        }

        _enemy.NavMeshAgent.isStopped = true;
        _enemy.NavMeshAgent.ResetPath();
        _enemy.NavMeshAgent.velocity = Vector3.zero;
        _enemy.NavMeshAgent.updatePosition = false;
        _enemy.NavMeshAgent.updateRotation = false;

        RotateTowardsTarget();

        return _isInAttackSequence ? NodeState.Running : NodeState.Success;
    }

    private void StartAttackSequence()
    {
        _isInAttackSequence = true;
        _attackSequenceTimer = 0f;
        _lastAttackTime = Time.time;

        _enemy.AttackPlayer();

        if (_enemy.Animator != null && _enemy.Animator.enabled)
        {
            _enemy.Animator.SetTrigger("Attack");
            _enemy.Animator.SetBool("IsRunning", false);
        }

        Debug.Log($"Enemy {_enemy.name} started attack sequence");
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
            _enemy.NavMeshAgent.updatePosition = true;
            _enemy.NavMeshAgent.updateRotation = true;
            _enemy.NavMeshAgent.isStopped = false;
        }
    }

    public bool IsAttacking()
    {
        return _isInAttackSequence;
    }
}