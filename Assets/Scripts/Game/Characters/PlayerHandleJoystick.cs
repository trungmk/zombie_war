using System;
using UnityEngine;

public class PlayerHandleJoystick : IHandleJoyStickDirection
{
    public Joystick JoystickType { get; }
    public Vector2 Direction { get; private set; }
    public bool IsActive { get; private set; }

    public Action<Vector2> OnDirectionChangedEvent;
    public Action OnDirectionEndedEvent;

    public PlayerHandleJoystick(Joystick joystickType)
    {
        JoystickType = joystickType;
        Direction = Vector2.zero;
        IsActive = false;
    }

    public void OnDirectionChanged(Vector2 direction)
    {
        Direction = direction;
        IsActive = direction.sqrMagnitude > 0.01f;
        OnDirectionChangedEvent?.Invoke(direction);
    }

    public void OnDirectionEnded()
    {
        Direction = Vector2.zero;
        IsActive = false;
        OnDirectionEndedEvent?.Invoke();
    }
}