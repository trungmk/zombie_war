using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour
{
    public int WaveIndex { get; private set; }
    public bool IsCompleted { get; private set; }

    public System.Action<GameObject> OnZombieSpawned;
    public System.Action OnWaveCompleted;

    private WaveData _waveData;
    private Transform[] _spawnPoints;
    private float _difficultyMultiplier;
    private float _spawnTimer;
    private int _zombiesSpawned;
    private bool _spawningActive = true;

    public void Initialize(WaveData waveData, Transform[] spawnPoints, float difficultyMultiplier)
    {
        _waveData = waveData;
        _spawnPoints = spawnPoints;
        _difficultyMultiplier = difficultyMultiplier;
        _spawnTimer = 0f;
        _zombiesSpawned = 0;
        IsCompleted = false;
        WaveIndex = transform.GetSiblingIndex();

        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        float waveEndTime = Time.time + _waveData.waveDuration;

        while (_spawningActive && _zombiesSpawned < _waveData.totalZombies && Time.time < waveEndTime)
        {
            if (_spawnTimer <= 0f)
            {
                SpawnZombie();
                _spawnTimer = _waveData.spawnInterval / _difficultyMultiplier; // Faster spawning with higher difficulty
            }

            _spawnTimer -= Time.deltaTime;
            yield return null;
        }

        CompleteWave();
    }

    private void SpawnZombie()
    {
        if (_spawnPoints.Length == 0 || _waveData.zombieTypes.Length == 0) return;

        // Choose zombie type based on probability
        ZombieSpawnData chosenZombieData = ChooseZombieType();
        if (chosenZombieData?.zombiePrefab == null) return;

        // Choose spawn point
        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        Vector3 spawnPosition = GetValidSpawnPosition(spawnPoint.position);

        // Spawn zombie
        GameObject zombie = Instantiate(chosenZombieData.zombiePrefab, spawnPosition, spawnPoint.rotation);

        // Apply wave-specific scaling
        ApplyWaveScaling(zombie);

        _zombiesSpawned++;
        OnZombieSpawned?.Invoke(zombie);
    }

    private ZombieSpawnData ChooseZombieType()
    {
        float totalProbability = 0f;
        List<ZombieSpawnData> availableTypes = new List<ZombieSpawnData>();

        // Filter available types and calculate total probability
        foreach (var zombieType in _waveData.zombieTypes)
        {
            if (zombieType.maxCount <= 0 || GetSpawnedCount(zombieType) < zombieType.maxCount)
            {
                availableTypes.Add(zombieType);
                totalProbability += zombieType.spawnProbability;
            }
        }

        if (availableTypes.Count == 0) return null;

        // Choose based on probability
        float randomValue = Random.Range(0f, totalProbability);
        float currentProbability = 0f;

        foreach (var zombieType in availableTypes)
        {
            currentProbability += zombieType.spawnProbability;
            if (randomValue <= currentProbability)
            {
                return zombieType;
            }
        }

        return availableTypes[0]; // Fallback
    }

    private int GetSpawnedCount(ZombieSpawnData zombieType)
    {
        // Count how many of this type have been spawned
        // This is a simplified version - you might want to track this more accurately
        return 0;
    }

    private Vector3 GetValidSpawnPosition(Vector3 basePosition)
    {
        Vector2 randomOffset = Random.insideUnitCircle * 2f;
        Vector3 spawnPosition = basePosition + new Vector3(randomOffset.x, 0, randomOffset.y);

        if (UnityEngine.AI.NavMesh.SamplePosition(spawnPosition, out UnityEngine.AI.NavMeshHit hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
        {
            return hit.position;
        }

        return basePosition;
    }

    private void ApplyWaveScaling(GameObject zombie)
    {
        //var healthComponent = zombie.GetComponent<HealthComponent>();
        //if (healthComponent != null)
        //{
        //    healthComponent.SetMaxHealth(healthComponent.MaxHealth * _waveData.healthMultiplier * _difficultyMultiplier);
        //}

        //var zombieController = zombie.GetComponent<ZombieController>();
        //if (zombieController != null)
        //{
        //    zombieController.SetSpeedMultiplier(_waveData.speedMultiplier * _difficultyMultiplier);
        //    zombieController.SetDamageMultiplier(_waveData.damageMultiplier * _difficultyMultiplier);
        //}
    }

    private void CompleteWave()
    {
        IsCompleted = true;
        _spawningActive = false;

        Debug.Log($"Wave completed! Spawned {_zombiesSpawned} zombies");
        OnWaveCompleted?.Invoke();

        // Clean up after a delay
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
    }

    internal void Initialize(WaveData waveData, Transform[] spawnPoints)
    {
        throw new System.NotImplementedException();
    }
}