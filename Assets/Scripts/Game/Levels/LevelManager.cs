using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField]
    private LevelData _currentLevelData;

    [SerializeField] 
    private Transform[] _spawnPoints;

    [SerializeField] 
    private float _spawnRadius = 2f;

    public Action<int> OnWaveStarted;

    public Action<int> OnWaveCompleted;

    public Action OnLevelCompleted;

    public Action OnLevelFailed;

    private float _levelTimer;

    private int _currentWaveIndex = 0;

    private List<WaveSpawner> _activeWaveSpawners = new List<WaveSpawner>();

    private bool _levelCompleted = false;

    private int _totalZombiesSpawned = 0;

    private int _totalZombiesKilled = 0;

    public LevelData CurrentLevelData => _currentLevelData;
    public int CurrentWave => _currentWaveIndex;
    public int TotalWaves => _currentLevelData.waves.Length;
    public int ZombiesKilled => _totalZombiesKilled;
    public int ZombiesSpawned => _totalZombiesSpawned;
    public bool IsLevelActive => !_levelCompleted;

    public bool IsStartLevel { get; set; }

    public void LoadLevel(LevelData levelData)
    {
        _currentLevelData = levelData;
        StartLevel();
    }

    private void Update()
    {
        if (_levelCompleted || _currentLevelData == null) 
        { 
            return; 
        }

        if (!IsStartLevel)
        {
            return;
        }

        _levelTimer += Time.deltaTime;

        HandleWaveSpawning();
    }

    private void StartLevel()
    {
        if (_currentLevelData == null)
        {
            return;
        }

        _levelTimer = 0f;
        _currentWaveIndex = 0;
        _levelCompleted = false;
        _totalZombiesSpawned = 0;
        _totalZombiesKilled = 0;

        Debug.Log($"Starting {_currentLevelData.levelName}!");
    }

    private void HandleWaveSpawning()
    {
        for (int i = _currentWaveIndex; i < _currentLevelData.waves.Length; i++)
        {
            WaveData wave = _currentLevelData.waves[i];

            if (_levelTimer >= wave.waveStartTime && !IsWaveActive(i))
            {
                StartWave(i);
                _currentWaveIndex = i + 1;
            }
        }

        _activeWaveSpawners.RemoveAll(spawner => spawner.IsCompleted);
    }

    private void StartWave(int waveIndex)
    {
        WaveData waveData = _currentLevelData.waves[waveIndex];

        GameObject spawnerObj = new GameObject($"Wave_{waveIndex + 1}_Spawner");
        spawnerObj.transform.SetParent(transform);

        WaveSpawner spawner = spawnerObj.AddComponent<WaveSpawner>();
        spawner.Initialize(waveData, _spawnPoints);
        spawner.OnZombieSpawned += OnZombieSpawned;
        spawner.OnWaveCompleted += () => OnWaveCompleted?.Invoke(waveIndex);

        _activeWaveSpawners.Add(spawner);
        OnWaveStarted?.Invoke(waveIndex);
    }

    private bool IsWaveActive(int waveIndex)
    {
        return _activeWaveSpawners.Exists(spawner => spawner.WaveIndex == waveIndex);
    }

    private void OnZombieSpawned(GameObject zombie)
    {
        _totalZombiesSpawned++;
    }

    private void OnZombieDied(GameObject zombie)
    {
        _totalZombiesKilled++;
    }

    private void CompleteLevel()
    {
        if (_levelCompleted) return;

        _levelCompleted = true;

        // Stop all spawners
        foreach (var spawner in _activeWaveSpawners)
        {
            if (spawner != null)
                spawner.StopSpawning();
        }

        OnLevelCompleted?.Invoke();
    }

    public void FailLevel()
    {
        if (_levelCompleted) return;

        _levelCompleted = true;

        // Stop all spawners
        foreach (var spawner in _activeWaveSpawners)
        {
            if (spawner != null)
                spawner.StopSpawning();
        }

        Debug.Log($"{_currentLevelData.levelName} Failed!");
        OnLevelFailed?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        // Draw spawn points and radius
        if (_spawnPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (var spawnPoint in _spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, _spawnRadius);
                    Gizmos.DrawWireCube(spawnPoint.position, Vector3.one * 0.5f);
                }
            }
        }
    }
}