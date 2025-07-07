using UnityEngine;

public class EnemyBehavior : BehaviorTree
{
    private Enemy _enemy;
    private Transform _currentTarget;
    private EnemyAttackNode _attackNode;
    private EnemyChaseNode _chaseNode;
    private EnemyPatrolNode _patrolNode;
    private string _lastActiveNode = "";

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
            _attackNode,
            _chaseNode,
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

        UpdateCurrentTarget();
        _attackNode.SetTarget(_currentTarget);
        _chaseNode.SetTarget(_currentTarget);

        string previousNode = _lastActiveNode;
        NodeState result = RootNode.Run();
        TrackActiveNode();
    }

    private void TrackActiveNode()
    {
        if (_currentTarget != null)
        {
            float distance = Vector3.Distance(_enemy.transform.position, _currentTarget.position);

            if (_attackNode.IsAttacking())
            {
                _lastActiveNode = "ATTACK (Sequence)";
            }
            else if (distance <= _enemy.EnemyData.AttackRange)
            {
                _lastActiveNode = "ATTACK (Range)";
            }
            else if (distance <= _enemy.EnemyData.ChaseRange)
            {
                _lastActiveNode = "CHASE";
            }
            else
            {
                _lastActiveNode = "PATROL";
            }
        }
        else
        {
            _lastActiveNode = "PATROL (No Target)";
        }
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

    public EnemyAttackNode GetAttackNode()
    {
        return _attackNode;
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