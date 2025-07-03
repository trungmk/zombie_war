//using System.Collections.Generic;
//using UnityEngine;

//public partial class CustomBehavior
//{
//    public NodeState IsEnemyInRange()
//    {
//        if (_gameManager != null && _gameManager.Enemy != null)
//        {
//            List<Unit> enemyUnits = _gameManager.GetEnemyUnits(_unit);
//            if (enemyUnits != null)
//            {
//                for (int i = 0; i < enemyUnits.Count; i++)
//                {
//                    float dist = Vector3.Distance(_unit.transform.position, enemyUnits[i].transform.position);
//                    if (_unit.UnitStats.AttackRange > dist && !enemyUnits[i].IsDead())
//                    {
//                        Ray ray = new Ray(_unit.transform.position, _unit.transform.forward);
//                        RaycastHit raycastHit;
//                        if (Physics.Raycast(ray, out raycastHit, _unit.UnitStats.AttackRange, LayerMask.GetMask("Environment")))
//                        {
//                            if (raycastHit.distance < dist)
//                            {
//                                continue;
//                            }
//                        }

//                        _currentTarget = enemyUnits[i];
//                        _unit.Model.AnimationController.SetAnimation(UnitAnimatorBoolParameter.Run, false);
//                        return NodeState.Success;
//                    }
//                }
//            }
//        }
//        _unit.Model.AnimationController.SetAnimation(UnitAnimatorBoolParameter.Attack, false);
//        return NodeState.Failure;
//    }

//    public NodeState AttackEnemy()
//    {
//        if (_currentTarget != null)
//        {
//            if (_currentTarget.IsDead())
//            {
//                _currentTarget = null;
//                _unit.Model.AnimationController.SetAnimation(UnitAnimatorBoolParameter.Attack, false);
//                return NodeState.Failure;
//            }
//            _unit.NavMeshAgent.isStopped = true;
//            _unit.transform.LookAt(_currentTarget.transform);
//            _unit.Model.AnimationController.SetAnimation(UnitAnimatorBoolParameter.Attack, true);
//            return NodeState.Success;
//        }
//        return NodeState.Failure;
//    }
//}