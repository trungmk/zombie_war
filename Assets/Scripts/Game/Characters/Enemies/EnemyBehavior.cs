using UnityEngine;

public class EnemyBehavior : BehaviorTree
{
    private Enemy _enemy;
    private Transform _currentTarget;

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
        var rootSelector = new SelectorNode(_attackNode, _chaseNode, _patrolNode);
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

        UpdateCurrentTarget();

        _attackNode.SetTarget(_currentTarget);
        _chaseNode.SetTarget(_currentTarget);

        base.Tick();
    }

    public EnemyAttackNode GetAttackNode()
    {
        return _attackNode;
    }

    private void UpdateCurrentTarget()
    {
        if (_enemy.PlayerTransform != null)
        {
            _currentTarget = _enemy.PlayerTransform;
        }
        else
        {
            _currentTarget = null;
        }
    }

    public void OnEnemyDied()
    {
        if (_enemy.NavMeshAgent != null && _enemy.NavMeshAgent.enabled)
        {
            _enemy.NavMeshAgent.isStopped = true;
            _enemy.NavMeshAgent.ResetPath();
            _enemy.NavMeshAgent.enabled = false;
        }

        _patrolNode?.ResetPatrolState();
    }

    public void OnEnemyRespawned()
    {
        _patrolNode?.ResetPatrolState();
    }
}