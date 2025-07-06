using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string EnemyName = "Zombie";
    public int MaxHealth = 100;
    public float MoveSpeed = 3.5f;
    public float RotationSpeed = 360f;

    public int AttackDamage = 20;
    public float AttackRange = 1.5f;
    public float AttackCooldown = 1.2f;

    public float ChaseRange = 10f;
    public float DetectionRange = 12f;
    public float PatrolSpeed = 2f;
    public float ChaseSpeed = 5f;

    public float WaypointStoppingDistance = 0.5f;
    public float WaypointWaitTime = 2f;

    public AudioClip AttackSound;
    public AudioClip DeathSound;
    public AudioClip[] IdleSounds;
    public AudioClip[] WalkSounds;
}