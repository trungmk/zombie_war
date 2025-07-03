using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }
    public Vector2 AimInput { get; private set; }
    public bool IsMoving => MovementInput.sqrMagnitude > 0.01f;
    public bool IsAiming => AimInput.sqrMagnitude > 0.01f;
    public bool ShootPressed { get; private set; }
    public bool GrenadePressed { get; private set; }

    public void SetMovement(Vector2 dir) => MovementInput = dir;
    public void SetAim(Vector2 dir) => AimInput = dir;
    public void SetShoot(bool pressed) => ShootPressed = pressed;
    public void SetGrenade(bool pressed) => GrenadePressed = pressed;

    public PlayerHandleJoystick LeftJoyStick { get; private set; }

    public PlayerHandleJoystick RightJoyStick { get; private set; }

    private void Awake()
    {
        LeftJoyStick = new PlayerHandleJoystick(Joystick.Left);
        RightJoyStick = new PlayerHandleJoystick(Joystick.Right);
    }

    public PlayerInputData GetInputData()
    {
        return new PlayerInputData(
            MovementInput,
            AimInput,
            IsMoving,
            IsAiming,
            ShootPressed,
            GrenadePressed
        );
    }

    public void OnLeftJoystick(Vector2 dir) => SetMovement(dir);

    public void OnRightJoystick(Vector2 dir, bool justPressed)
    {
        SetAim(dir);
        SetShoot(justPressed);
    }
    public void OnGrenadeButton() => SetGrenade(true);

    private void LateUpdate()
    {
        // Reset one-frame actions
        SetShoot(false);
        SetGrenade(false);
    }
}