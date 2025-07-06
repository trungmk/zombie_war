using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementComponent : BaseComponent
{
    [SerializeField]
    private Enemy _enemy;

    private NavMeshAgent _navMeshAgent;
    private Rigidbody _rigidbody;

    private float _currentSpeed;
    private bool _isMoving;

    public bool IsMoving => _isMoving;
    public Vector3 CurrentVelocity => _navMeshAgent.velocity;

    public void Setup()
    {
        if (_navMeshAgent == null)
        {
            _navMeshAgent = _enemy.NavMeshAgent;
        }

        if (_rigidbody == null)
        {
            _rigidbody = _enemy.Rigidbody;
        }

        _navMeshAgent.angularSpeed = _enemy.EnemyData.RotationSpeed;
        SetSpeed(_enemy.EnemyData.PatrolSpeed);
    }

    public void SetSpeed(float speed)
    {
        _currentSpeed = speed;
        if (_navMeshAgent != null)
        {
            _navMeshAgent.speed = speed;
        }
    }

    public void MoveTo(Vector3 destination)
    {
        if (_navMeshAgent != null)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(destination);
            _isMoving = true;
        }
    }

    public void StopMovement()
    {
        if (_navMeshAgent != null)
        {
            _navMeshAgent.isStopped = true;
            _isMoving = false;
        }
    }

    public bool HasReachedDestination()
    {
        if (_navMeshAgent == null)
        {
            return false;
        } 
            
        return !_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;
    }

    public void LookAt(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}