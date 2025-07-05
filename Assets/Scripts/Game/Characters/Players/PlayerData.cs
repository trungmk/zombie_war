using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    public int MaxHealth = 100;

    public float MoveSpeed = 5f;

    public float RotationSpeed = 720f;

    public int MaxGrenades = 10;
}