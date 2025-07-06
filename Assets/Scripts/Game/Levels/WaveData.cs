using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Game/Wave Data")]
public class WaveData : ScriptableObject
{
    public float waveStartTime = 0f;

    public float waveDuration = 30f;

    public int totalZombies = 10;

    public float spawnInterval = 2f;

    public ZombieSpawnData[] zombieTypes;
}

[System.Serializable]
public class ZombieSpawnData
{
    public EnemyType enemyType;

    public int maxCount = 5;
}