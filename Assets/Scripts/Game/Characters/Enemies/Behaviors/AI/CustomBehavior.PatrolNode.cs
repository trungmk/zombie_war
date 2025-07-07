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
        if (_enemy == null || _enemy.HealthComponent == null || _enemy.HealthComponent.IsDead)
        {
            return NodeState.Failure;
        }

        if (_enemy.NavMeshAgent == null || !_enemy.NavMeshAgent.enabled || !_enemy.HasValidNavMesh())
        {
            return NodeState.Failure;
        }

        if (_enemy.Waypoints == null || _enemy.Waypoints.Length == 0)
        {
            return NodeState.Failure;
        }

        _enemy.NavMeshAgent.speed = _enemy.EnemyData.PatrolSpeed;
        if (_enemy.Animator != null && _enemy.Animator.enabled)
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

            return NodeState.Running;
        }

        if (_currentWaypointIndex >= _enemy.Waypoints.Length)
        {
            _currentWaypointIndex = 0;
        }

        Transform currentWaypoint = _enemy.Waypoints[_currentWaypointIndex];
        if (currentWaypoint == null)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _enemy.Waypoints.Length;
            return NodeState.Running;
        }

        if (_enemy.NavMeshAgent.isStopped)
        {
            _enemy.NavMeshAgent.isStopped = false;
        }

        _enemy.NavMeshAgent.SetDestination(currentWaypoint.position);

        if (!_enemy.NavMeshAgent.pathPending &&
            _enemy.NavMeshAgent.remainingDistance <= _enemy.NavMeshAgent.stoppingDistance)
        {
            _isWaitingAtWaypoint = true;
            _waypointWaitTimer = 0f;
        }

        return NodeState.Running;
    }

    public void ResetPatrolState()
    {
        _currentWaypointIndex = 0;
        _waypointWaitTimer = 0f;
        _isWaitingAtWaypoint = false;
    }
}