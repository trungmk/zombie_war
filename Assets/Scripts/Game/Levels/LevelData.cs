using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber = 1;
    public string levelName = "Level 1";

    [Header("Level Settings")]
    public WaveData[] waves;

    [Header("Rewards")]
    public int coinsReward = 100;
    public int experienceReward = 50;
}