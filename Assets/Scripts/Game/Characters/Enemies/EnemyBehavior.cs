using UnityEngine;

public class EnemyBehavior : BehaviorTree
{
    private Enemy _enemy;
    private Transform _currentTarget;

    // Node references
    private EnemyAttackNode _attackNode;
    private EnemyChaseNode _chaseNode;
    private EnemyPatrolNode _patrolNode;

    public EnemyBehavior(Enemy enemy)
    {
        _enemy = enemy;
        SetupBehaviorTree();
    }

    private void SetupBehaviorTree()
    {
        _attackNode = new EnemyAttackNode(_enemy, _currentTarget);
        _chaseNode = new EnemyChaseNode(_enemy, _currentTarget);
        _patrolNode = new EnemyPatrolNode(_enemy);

        var rootSelector = new SelectorNode(
            new SequenceNode(
                new LeafNode(IsPlayerInAttackRange),
                _attackNode
            ),
            new SequenceNode(
                new LeafNode(IsPlayerInChaseRange),
                _chaseNode
            ),
            _patrolNode
        );

        RootNode = rootSelector;
    }

    public override void Tick()
    {
        if (_enemy == null || _enemy.HealthComponent == null || _enemy.HealthComponent.IsDead)
        {
            return;
        }

        if (_enemy.NavMeshAgent == null || !_enemy.NavMeshAgent.enabled)
        {
            return;
        }

        base.Tick();
    }

    private NodeState IsPlayerInAttackRange()
    {
        if (_enemy == null || _enemy.HealthComponent == null || _enemy.HealthComponent.IsDead)
        {
            return NodeState.Failure;
        }

        if (_enemy.PlayerTransform == null)
        {
            return NodeState.Failure;
        }

        float distance = Vector3.Distance(_enemy.transform.position, _enemy.PlayerTransform.position);
        if (distance <= _enemy.EnemyData.AttackRange)
        {
            _currentTarget = _enemy.PlayerTransform;
            _attackNode.SetTarget(_currentTarget);
            return NodeState.Success;
        }

        return NodeState.Failure;
    }

    private NodeState IsPlayerInChaseRange()
    {
        if (_enemy == null || _enemy.HealthComponent == null || _enemy.HealthComponent.IsDead)
        {
            return NodeState.Failure;
        }

        if (_enemy.PlayerTransform == null)
        {
            return NodeState.Failure;
        }

        float distance = Vector3.Distance(_enemy.transform.position, _enemy.PlayerTransform.position);

        if (distance <= _enemy.EnemyData.ChaseRange && distance > _enemy.EnemyData.AttackRange)
        {
            _currentTarget = _enemy.PlayerTransform;
            _chaseNode.SetTarget(_currentTarget);
            return NodeState.Success;
        }

        return NodeState.Failure;
    }

    public void OnEnemyDied()
    {
        if (_enemy.NavMeshAgent != null && _enemy.NavMeshAgent.enabled)
        {
            _enemy.NavMeshAgent.isStopped = true;
            _enemy.NavMeshAgent.ResetPath();
        }

        _patrolNode?.ResetPatrolState();
    }

    public void OnEnemyRespawned()
    {
        _patrolNode?.ResetPatrolState();
    }
}