using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }

    public Vector2 AimInput { get; private set; }

    public bool IsMoving => MovementInput.sqrMagnitude > 0.01f;

    public bool IsAiming => AimInput.sqrMagnitude > 0.01f;

    public bool ShootPressed { get; private set; }

    public bool GrenadePressed { get; private set; }

    public void SetMovement(Vector2 dir)
    {
        MovementInput = dir;
    } 
        
    public void SetAim(Vector2 dir)
    {
        AimInput = dir;
    }    

    public void SetShoot(bool pressed)
    {
        ShootPressed = pressed;
    }    

    public void SetGrenade(bool pressed)
    {
        GrenadePressed = pressed;
    }    

    public PlayerHandleJoystick LeftJoyStick { get; private set; }

    public PlayerHandleJoystick RightJoyStick { get; private set; }

    private PlayerInputData _cachedInputData;

    private bool _inputDataDirty = true;

    private void Awake()
    {
        LeftJoyStick = new PlayerHandleJoystick(Joystick.Left);
        LeftJoyStick.OnDirectionChangedEvent += OnLeftJoystickChangeDirecion;
        LeftJoyStick.OnDirectionEndedEvent += OnLeftJoystickEndEvent;

        RightJoyStick = new PlayerHandleJoystick(Joystick.Right);
        RightJoyStick.OnDirectionChangedEvent = OnRightJoystickChangeDirecion;
        RightJoyStick.OnDirectionEndedEvent = OnRightJoystickEndEvent;
        RightJoyStick.OnStartClickedEvent = OnRightJoystickStartClickedEvent;
    }

    private void OnRightJoystickStartClickedEvent(Vector2 dir)
    {
        SetAim(Vector2.one);
        SetShoot(true);
        _inputDataDirty = true;
    }

    private void OnRightJoystickEndEvent()
    {
        SetAim(Vector2.zero);
        SetShoot(false);
        _inputDataDirty = true;
    }

    private void OnLeftJoystickEndEvent()
    {
        SetMovement(Vector2.zero);
        _inputDataDirty = true;
    }

    public PlayerInputData GetInputData()
    {
        if (_inputDataDirty)
        {
            _cachedInputData = new PlayerInputData(
                MovementInput,
                AimInput,
                IsMoving,
                ShootPressed,
                GrenadePressed
            );

            _inputDataDirty = false;
        }

        return _cachedInputData;
    }

    public void OnLeftJoystickChangeDirecion(Vector2 dir)
    {
        SetMovement(dir);
        _inputDataDirty = true;
    } 
        

    public void OnRightJoystickChangeDirecion(Vector2 dir)
    {
        SetAim(dir);
        SetShoot(true);
        _inputDataDirty = true;
    }

    public void OnGrenadeButton()
    {
        SetGrenade(true);
        _inputDataDirty = true;
    }

    public void ResetGrenadeInput()
    {
        GrenadePressed = false;
        _inputDataDirty = true;
    }
}