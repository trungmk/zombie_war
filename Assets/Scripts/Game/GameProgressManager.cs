using System;
using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    [SerializeField] 
    private LevelData[] _allLevels;

    [SerializeField] 
    private int _currentLevelIndex = 0;

    public static GameProgressManager Instance { get; private set; }

    public Action<LevelData> OnLevelChanged;

    public Action<int> OnProgressSaved;

    public LevelData CurrentLevel => _allLevels[_currentLevelIndex];
    public int CurrentLevelIndex => _currentLevelIndex;
    public int TotalLevels => _allLevels.Length;
    public bool HasNextLevel => _currentLevelIndex < _allLevels.Length - 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadProgress();
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if(_allLevels == null || _allLevels.Length < 0)
        {
            return;
        }

        if (levelIndex >= 0 && levelIndex < _allLevels.Length)
        {
            _currentLevelIndex = levelIndex;
            OnLevelChanged?.Invoke(CurrentLevel);
        }
    }

    public void NextLevel()
    {
        if (HasNextLevel)
        {
            _currentLevelIndex++;
            SaveProgress();
            OnLevelChanged?.Invoke(CurrentLevel);
        }
    }

    public void RestartCurrentLevel()
    {
        OnLevelChanged?.Invoke(CurrentLevel);
    }

    public void UnlockNextLevel()
    {
        SaveProgress();
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("CurrentLevel", _currentLevelIndex);
        PlayerPrefs.Save();
        OnProgressSaved?.Invoke(_currentLevelIndex);
    }

    private void LoadProgress()
    {
        _currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel", 0);
    }

    public LevelData GetLevel(int index)
    {
        if (index >= 0 && index < _allLevels.Length)
            return _allLevels[index];
        return null;
    }
}