using UnityEngine;

public struct PlayerInputData
{
    public Vector2 MovementInput;
    public Vector2 AimingInput;
    public bool IsMoving;
    public bool ShootTriggered;
    public bool GrenadeTriggered;

    public PlayerInputData(Vector2 movement, Vector2 aim, bool moving, bool shoot, bool grenade)
    {
        MovementInput = movement;
        AimingInput = aim;
        IsMoving = moving;
        ShootTriggered = shoot;
        GrenadeTriggered = grenade;
    }
}