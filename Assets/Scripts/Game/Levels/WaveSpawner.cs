using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using MEC;
using System;

public class WaveSpawner : MonoBehaviour
{
    public int WaveIndex { get; private set; }
    public bool IsCompleted { get; private set; }

    public Action<GameObject> OnZombieSpawned;

    public Action OnWaveCompleted;

    private WaveData _waveData;

    private Transform[] _spawnPoints;

    private float _spawnTimer;

    private int _zombiesSpawned;

    private bool _spawningActive = true;

    private Dictionary<EnemyType, int> _spawnedCountByType = new Dictionary<EnemyType, int>();

    public void Initialize(WaveData waveData, Transform[] spawnPoints)
    {
        _waveData = waveData;
        _spawnPoints = spawnPoints;
        _spawnTimer = 0f;
        _zombiesSpawned = 0;
        IsCompleted = false;
        WaveIndex = transform.GetSiblingIndex();

        _spawnedCountByType.Clear();
        foreach (var zombieType in _waveData.zombieTypes)
        {
            _spawnedCountByType[zombieType.enemyType] = 0;
        }

        Timing.RunCoroutine(SpawnWave());
    }

    private IEnumerator<float> SpawnWave()
    {
        float waveEndTime = Time.time + _waveData.waveDuration;

        while (_spawningActive && _zombiesSpawned < _waveData.totalZombies && Time.time < waveEndTime)
        {
            if (_spawnTimer <= 0f)
            {
                SpawnZombieAsync().Forget();
                _spawnTimer = _waveData.spawnInterval;
            }

            _spawnTimer -= Time.deltaTime;
            yield return Timing.WaitForOneFrame;
        }

        CompleteWave();
    }

    private async UniTaskVoid SpawnZombieAsync()
    {
        if (_spawnPoints.Length == 0 || _waveData.zombieTypes.Length == 0)
        {
            return;
        }

        ZombieSpawnData chosenZombieData = ChooseZombieType();

        if (chosenZombieData == null)
        {
            return;
        }

        Transform spawnPoint = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Length)];
        Vector3 spawnPosition = GetValidSpawnPosition(spawnPoint.position);
        string enemyAddressName = GetEnemyAddressName(chosenZombieData.enemyType);
        GameObject enemy = await ObjectPooling.Instance.Get(enemyAddressName, PoolItemType.Enemy);

        if (enemy != null)
        {
            enemy.transform.position = spawnPosition;
            enemy.transform.rotation = Quaternion.identity;

            var enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.Initialize();
                InGamePanel inGamePanel = UIManager.Instance.GetCache<InGamePanel>();
                if (inGamePanel != null)
                {
                    inGamePanel.RegisterEnemy(enemyComponent);
                }
            }

            _zombiesSpawned++;
            _spawnedCountByType[chosenZombieData.enemyType]++;

            OnZombieSpawned?.Invoke(enemy);
        }
    }

    private string GetEnemyAddressName(EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.ZombieLight => "Zombie_Light",
            _ => "Zombie_Light" // Default fallback
        };
    }

    private ZombieSpawnData ChooseZombieType()
    {
        List<ZombieSpawnData> availableTypes = new List<ZombieSpawnData>();

        foreach (var zombieType in _waveData.zombieTypes)
        {
            int currentCount = _spawnedCountByType.GetValueOrDefault(zombieType.enemyType, 0);
            if (zombieType.maxCount <= 0 || currentCount < zombieType.maxCount)
            {
                availableTypes.Add(zombieType);
            }
        }

        if (availableTypes.Count == 0)
        {
            Debug.LogWarning("No available zombie types to spawn (all reached max count)");
            return null;
        }

        return availableTypes[UnityEngine.Random.Range(0, availableTypes.Count)];
    }

    private int GetSpawnedCount(ZombieSpawnData zombieType)
    {
        return _spawnedCountByType.GetValueOrDefault(zombieType.enemyType, 0);
    }

    private Vector3 GetValidSpawnPosition(Vector3 basePosition)
    {
        for (int i = 0; i < 5; i++)
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * 2f;
            Vector3 spawnPosition = basePosition + new Vector3(randomOffset.x, 0, randomOffset.y);

            if (UnityEngine.AI.NavMesh.SamplePosition(spawnPosition, out UnityEngine.AI.NavMeshHit hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return basePosition;
    }

    private void CompleteWave()
    {
        IsCompleted = true;
        _spawningActive = false;

        OnWaveCompleted?.Invoke();

        StartCoroutine(CleanupAfterDelay(5f));
    }

    private IEnumerator CleanupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    public void StopSpawning()
    {
        _spawningActive = false;
        StopAllCoroutines();
        Debug.Log($"Wave {WaveIndex} spawning stopped. Spawned {_zombiesSpawned} zombies.");
    }

    [ContextMenu("Force Complete Wave")]
    public void ForceCompleteWave()
    {
        CompleteWave();
    }

    [ContextMenu("Spawn One Zombie")]
    public void SpawnOneZombieTest()
    {
        if (_spawningActive)
        {
            SpawnZombieAsync().Forget();
        }
    }

    // Get current spawn statistics
    public Dictionary<EnemyType, int> GetSpawnStatistics()
    {
        return new Dictionary<EnemyType, int>(_spawnedCountByType);
    }

    public float GetSpawnProgress()
    {
        return _waveData.totalZombies > 0 ? (float)_zombiesSpawned / _waveData.totalZombies : 0f;
    }
}