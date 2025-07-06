using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Enemy _enemy;
    private EnemyBehavior _behaviorTree;

    public void Initialize(Enemy enemy)
    {
        _enemy = enemy;
        _behaviorTree = new EnemyBehavior(enemy);
    }

    private void Update()
    {
        if (_behaviorTree != null)
        {
            _behaviorTree.Tick();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_enemy == null || _enemy.EnemyData == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _enemy.EnemyData.AttackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _enemy.EnemyData.ChaseRange);
    }
}