using UnityEngine;

public class EnemyChaseNode : LeafNode
{
    private Enemy _enemy;

    private Transform _currentTarget;

    public EnemyChaseNode(Enemy enemy, Transform currentTarget) : base(null)
    {
        _enemy = enemy;
        _currentTarget = currentTarget;
        SetFunction(ChasePlayer);
    }

    public void SetTarget(Transform target)
    {
        _currentTarget = target;
    }

    private NodeState ChasePlayer()
    {
        if (_currentTarget == null)
            return NodeState.Failure;

        float distance = Vector3.Distance(_enemy.transform.position, _currentTarget.position);
        if (distance <= _enemy.EnemyData.AttackRange)
        {
            return NodeState.Failure;
        }

        if (_enemy.NavMeshAgent.isStopped)
        {
            _enemy.NavMeshAgent.isStopped = false;
        }

        _enemy.NavMeshAgent.speed = _enemy.EnemyData.ChaseSpeed;
        _enemy.NavMeshAgent.SetDestination(_currentTarget.position);

        if (_enemy.Animator != null)
        {
            _enemy.Animator.SetBool("IsRunning", true);
        }

        return NodeState.Success;
    }
}