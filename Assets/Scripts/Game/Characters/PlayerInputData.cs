using UnityEngine;

public struct PlayerInputData
{
    public Vector2 movementInput;
    public Vector2 aimInput;
    public bool isMoving;
    public bool isAiming;
    public bool shootTriggered;
    public bool grenadeTriggered;

    public PlayerInputData(Vector2 movement, Vector2 aim, bool moving, bool aiming, bool shoot, bool grenade)
    {
        movementInput = movement;
        aimInput = aim;
        isMoving = moving;
        isAiming = aiming;
        shootTriggered = shoot;
        grenadeTriggered = grenade;
    }
}