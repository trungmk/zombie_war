using UnityEngine;

public struct PlayerInputData
{
    public Vector2 MovementInput;
    public Vector2 AimInput;
    public bool IsMoving;
    public bool IsAiming;
    public bool ShootTriggered;
    public bool GrenadeTriggered;

    public PlayerInputData(Vector2 movement, Vector2 aim, bool moving, bool aiming, bool shoot, bool grenade)
    {
        MovementInput = movement;
        AimInput = aim;
        IsMoving = moving;
        IsAiming = aiming;
        ShootTriggered = shoot;
        GrenadeTriggered = grenade;
    }
}