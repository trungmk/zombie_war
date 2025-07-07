using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Enemy _enemy;
    private EnemyBehavior _behaviorTree;

    public EnemyBehavior BehaviorTree => _behaviorTree;

    public void Initialize(Enemy enemy)
    {
        _enemy = enemy;
        _behaviorTree = new EnemyBehavior(enemy);

        if (_enemy.HealthComponent != null)
        {
            _enemy.HealthComponent.OnDied += OnEnemyDied;
        }
    }

    private void Update()
    {
        if (_behaviorTree != null)
        {
            _behaviorTree.Tick();
        }
    }

    private void OnEnemyDied()
    {
        _behaviorTree?.OnEnemyDied();
    }

    private void OnDestroy()
    {
        if (_enemy != null && _enemy.HealthComponent != null)
        {
            _enemy.HealthComponent.OnDied -= OnEnemyDied;
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