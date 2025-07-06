using UnityEngine;

public class EnemyPatrolNode : LeafNode
{
    private Enemy _enemy;
    private int _currentWaypointIndex = 0;
    private float _waypointWaitTimer = 0f;
    private bool _isWaitingAtWaypoint = false;

    private const string IS_RUNNING_PARAM = "IsRunning";

    public EnemyPatrolNode(Enemy enemy) : base(null)
    {
        _enemy = enemy;
        SetFunction(PatrolWaypoints);
    }

    private NodeState PatrolWaypoints()
    {
        if (_enemy.Waypoints == null || _enemy.Waypoints.Length == 0)
        {
			return NodeState.Failure;
		}

        _enemy.NavMeshAgent.speed = _enemy.EnemyData.PatrolSpeed;

        if (_enemy.Animator != null)
        {
            _enemy.Animator.SetBool(IS_RUNNING_PARAM, false);
        }

        if (_isWaitingAtWaypoint)
        {
            _waypointWaitTimer += Time.deltaTime;
            if (_waypointWaitTimer >= _enemy.EnemyData.WaypointWaitTime)
            {
                _isWaitingAtWaypoint = false;
                _waypointWaitTimer = 0f;
                _currentWaypointIndex = (_currentWaypointIndex + 1) % _enemy.Waypoints.Length;
            }

            return NodeState.Success;
        }

        _enemy.NavMeshAgent.isStopped = false;
        _enemy.NavMeshAgent.SetDestination(_enemy.Waypoints[_currentWaypointIndex].position);

        if (!_enemy.NavMeshAgent.pathPending &&
            _enemy.NavMeshAgent.remainingDistance <= _enemy.NavMeshAgent.stoppingDistance)
        {
            _isWaitingAtWaypoint = true;
        }

        return NodeState.Success;
    }
}