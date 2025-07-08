using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber = 1;
    public string levelName = "Level 1";
    public WaveData[] waves;

    public int coinsReward = 100;
    public int experienceReward = 50;
}