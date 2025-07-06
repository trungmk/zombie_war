using UnityEngine;

public class EnemyCheckRangeNode : LeafNode
{
    private Enemy _enemy;
    private float _range;
    private System.Action<Transform> _onTargetFound;

    public EnemyCheckRangeNode(Enemy enemy, float range, System.Action<Transform> onTargetFound) : base(null)
    {
        _enemy = enemy;
        _range = range;
        _onTargetFound = onTargetFound;
        SetFunction(CheckPlayerInRange);
    }

    private NodeState CheckPlayerInRange()
    {
        if (_enemy.PlayerTransform == null)
        {
			return NodeState.Failure;
		}

        float distance = Vector3.Distance(_enemy.transform.position, _enemy.PlayerTransform.position);
        if (distance <= _range)
        {
            _onTargetFound?.Invoke(_enemy.PlayerTransform);
            return NodeState.Success;
        }

        return NodeState.Failure;
    }
}