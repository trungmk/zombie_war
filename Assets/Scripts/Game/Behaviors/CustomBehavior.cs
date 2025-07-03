//using Knight;
//using System.Collections.Generic;

//public partial class CustomBehavior : BehaviorTree, IUnitBehavior
//{
//    private Unit _unit;
//    private readonly GameManager _gameManager;
//    private Unit _currentTarget = null;
//    private int _currentPos = 0;
//    private WayPoint _currentWayPoint = null;
//    public CustomBehavior(Unit unit, GameManager gameManager)
//    {
//        _gameManager = gameManager;
//        _unit = unit;
//        List<WayPoint> wayPoints = _gameManager.MapManager.CurrentMap.WayPoints;
//        if (wayPoints.Count > 0)
//        {
//            int random = Random.Range(0, 100);
//            _currentWayPoint = wayPoints[random % wayPoints.Count];
//        }
//        SelectorNode rootNode = new SelectorNode();

//        Node attackskill1 = CreateAttackNodeInternal(/*_unit.UnitDefinition.AIAttackType1*/);
//        if (attackskill1 != null)
//        {
//            rootNode.AddChild(attackskill1);
//        }

//        Node chaseNode = CreateChaseNodeInternal(/*_unit.UnitDefinition.AIChaseType*/);
//        if (chaseNode != null)
//        {
//            rootNode.AddChild(chaseNode);
//        }

//        Node patrolNode = CreatePatrolNodeInternal(/*_unit.UnitDefinition.AIPatrolType*/);
//        if (patrolNode != null)
//        {
//            rootNode.AddChild(patrolNode);
//        }
//        //var rootNode = new SelectorNode(
//        //    new SequenceNode(
//        //        new LeafNode(IsEnemyInRange),
//        //        new LeafNode(AttackEnemy)
//        //        ),
//        //    new LeafNode(TryChaseTarget),
//        //    new LeafNode(DoPartrolWaypoint)
//        //    );
//        RootNode = rootNode;
//    }

//    private Node CreateAttackNodeInternal(/*AIAttackType attacktype*/)
//    {
//        return new SequenceNode(
//                new LeafNode(IsEnemyInRange),
//                new LeafNode(AttackEnemy));
//        //switch (attacktype)
//        //{
//        //    case AIAttackType.Basic:
//        //        return new SequenceNode(
//        //        new LeafNode(IsEnemyInRange),
//        //        new LeafNode(AttackEnemy));
//        //}
//        //return null;
//    }

//    private Node CreateChaseNodeInternal(/*AIChaseType chaseType*/)
//    {
//        return new LeafNode(TryChaseTarget);
//        //switch (chaseType)
//        //{
//        //    case AIChaseType.Normal:
//        //        return new LeafNode(TryChaseTarget);
//        //}
//        //return null;
//    }

//    private Node CreatePatrolNodeInternal(/*AIPatrolType patrolType*/)
//    {
//        return new LeafNode(DoPartrolWaypoint);
//        //switch (patrolType)
//        //{
//        //    case AIPatrolType.WayPoints:
//        //        return new LeafNode(DoPartrolWaypoint);
//        //}
//        //return null;
//    }

//}