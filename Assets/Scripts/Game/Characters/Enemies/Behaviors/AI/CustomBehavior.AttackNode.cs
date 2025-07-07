using UnityEngine;

public class EnemyAttackNode : LeafNode
{
    private Enemy _enemy;
    private Transform _currentTarget;
    private float _lastAttackTime = -999f;
    private bool _isInAttackSequence = false;

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
            _isInAttackSequence = false;
            return NodeState.Failure;
        }

        if (_enemy.NavMeshAgent == null || !_enemy.NavMeshAgent.enabled)
        {
            _isInAttackSequence = false;
            return NodeState.Failure;
        }

        if (_currentTarget == null)
        {
            _isInAttackSequence = false;
            return NodeState.Failure;
        }

        float distanceToPlayer = Vector3.Distance(_enemy.transform.position, _currentTarget.position);
        if (distanceToPlayer > _enemy.EnemyData.AttackRange)
        {
            _isInAttackSequence = false;
            return NodeState.Failure;
        }

        _enemy.NavMeshAgent.isStopped = true;
        _enemy.NavMeshAgent.velocity = Vector3.zero;
        _enemy.NavMeshAgent.SetDestination(_enemy.transform.position);

        Vector3 direction = (_currentTarget.position - _enemy.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _enemy.transform.rotation = Quaternion.Lerp(_enemy.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        if (Time.time - _lastAttackTime >= _enemy.EnemyData.AttackCooldown)
        {
            _isInAttackSequence = true;
            _enemy.AttackPlayer();
            _lastAttackTime = Time.time;

            if (_enemy.Animator != null && _enemy.Animator.enabled)
            {
                _enemy.Animator.SetTrigger("Attack");
                _enemy.Animator.SetBool("IsRunning", false);
            }
        }

        if (_isInAttackSequence && Time.time - _lastAttackTime < 1f)
        {
            return NodeState.Running;
        }

        _isInAttackSequence = false;
        return NodeState.Success;
    }
}