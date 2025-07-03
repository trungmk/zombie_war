//public partial class CustomBehavior
//{
//    public NodeState DoPartrolWaypoint()
//    {
//        if (_currentWayPoint != null)
//        {
//            _unit.NavMeshAgent.SetDestination(_currentWayPoint.Points[_currentPos]);

//            if (_unit.NavMeshAgent.isStopped)
//            {
//                _unit.NavMeshAgent.isStopped = false;
//            }

//            if (_unit.NavMeshAgent.remainingDistance <= _unit.NavMeshAgent.stoppingDistance
//                && !_unit.NavMeshAgent.pathPending)
//            {
//                _currentPos = (_currentPos + 1) % _currentWayPoint.Points.Count;
//            }
//        }

//        return NodeState.Success;
//    }
//}