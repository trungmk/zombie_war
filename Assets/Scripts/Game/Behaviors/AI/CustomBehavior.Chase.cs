//using System.Collections.Generic;

//public partial class CustomBehavior
//{
//    public NodeState TryChaseTarget()
//    {
//        if (_gameManager != null && _gameManager.Enemy != null)
//        {
//            List<Unit> enemyUnits = _gameManager.GetEnemyUnits(_unit);
//            if (enemyUnits != null)
//            {
//                for (int i = 0; i < enemyUnits.Count; i++)
//                {
//                    float dist = Vector3.Distance(_unit.transform.position, enemyUnits[i].transform.position);
//                    if (_unit.UnitStats.ChaseRange > dist && !enemyUnits[i].IsDead())
//                    {
//                        _unit.NavMeshAgent.destination = enemyUnits[i].transform.position;
//                        _unit.NavMeshAgent.isStopped = false;
//                        _unit.Model.AnimationController.SetAnimation(UnitAnimatorBoolParameter.Run, true);
//                        return NodeState.Success;
//                    }
//                }
//            }
//        }
//        _unit.Model.AnimationController.SetAnimation(UnitAnimatorBoolParameter.Run, false);
//        return NodeState.Failure;
//    }
//}