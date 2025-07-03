using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum Joystick : byte
{
    Left = 0,
    Right
}

public class JoyStickHandler : MonoBehaviour, ITouchTarget
{
    private readonly Dictionary<Joystick, List<IHandleJoyStickDirection>> _joyStickDirections = new Dictionary<Joystick, List<IHandleJoyStickDirection>>();

    private readonly Dictionary<int, Joystick> _touchIdToJoystick = new();

    private readonly Dictionary<int, Vector2> _touchIdToInitialPosition = new();

    private const float SCREEN_DIVISION_RATIO = 0.5f;

    private const float MAX_MAGNITUDE = 1f;

    public bool IsActive { get; private set; }

    private void OnEnable()
    {
        IsActive = true;
    }

    private void OnDisable()
    {
        IsActive = false;
    }

    private void OnDestroy()
    {
        if (_joyStickDirections != null)
        {
            _joyStickDirections.Clear();
        }
    }

    public void SetActive(bool active)
    {
        IsActive = active;
        if (!active)
        {
            _touchIdToJoystick.Clear();
            _touchIdToInitialPosition.Clear();
        }
    }

    public void RegisterHandleJoyStickDirection(Joystick joystick, IHandleJoyStickDirection handleJoyStickDirection)
    {
        if (handleJoyStickDirection == null)
        {
            Debug.LogError("HandleJoyStickDirection cannot be null.");
            return;
        }

        if (!_joyStickDirections.ContainsKey(joystick))
        {
            List<IHandleJoyStickDirection> handleJoyStickDirectionList = new List<IHandleJoyStickDirection>
            {
                handleJoyStickDirection
            };

            _joyStickDirections.Add(joystick, handleJoyStickDirectionList);
        }
        else
        {
            if(_joyStickDirections.TryGetValue(joystick, out List<IHandleJoyStickDirection> handleJoyStickDirectionList))
            {
                if (!handleJoyStickDirectionList.Contains(handleJoyStickDirection))
                {
                    handleJoyStickDirectionList.Add(handleJoyStickDirection);
                }
            }
        }
    }

    public void UnregisterHandleJoyStickDirection(Joystick joystick, IHandleJoyStickDirection handleJoyStickDirection)
    {
        if (handleJoyStickDirection == null)
        {
            Debug.LogError("HandleJoyStickDirection cannot be null.");
            return;
        }
        if (_joyStickDirections.TryGetValue(joystick, out List<IHandleJoyStickDirection> handleJoyStickDirectionList))
        {
            if (handleJoyStickDirectionList.Contains(handleJoyStickDirection))
            {
                handleJoyStickDirectionList.Remove(handleJoyStickDirection);
            }
        }
    }

    public void Dragged(InputData inputData)
    {
        if (!IsActive)
        {
            return;
        }

        if (_touchIdToJoystick.TryGetValue(inputData.TouchId, out Joystick joystick) &&
            _touchIdToInitialPosition.TryGetValue(inputData.TouchId, out Vector2 initialPosition))
        {
            Vector2 direction = CalculateDirection(initialPosition, inputData.TouchPosition);
            if (_joyStickDirections.TryGetValue(joystick, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    handler.OnDirectionChanged(direction);
                }
            }
        }
    }

    public void TouchedDown(InputData inputData)
    {
        if (!IsActive)
        {
            return;
        }    

        Joystick joystick = GetJoystickByPosition(inputData.TouchPosition);
        _touchIdToJoystick[inputData.TouchId] = joystick;
        _touchIdToInitialPosition[inputData.TouchId] = inputData.TouchPosition;
    }

    public void TouchedUp(InputData inputData)
    {
        if (!IsActive)
        {
            return;
        }

        if (_touchIdToJoystick.TryGetValue(inputData.TouchId, out Joystick joystick))
        {
            if (_joyStickDirections.TryGetValue(joystick, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    handler.OnDirectionEnded();
                }
            }

            _touchIdToJoystick.Remove(inputData.TouchId);
            _touchIdToInitialPosition.Remove(inputData.TouchId);
        }
    }

    private Joystick GetJoystickByPosition(Vector2 screenPosition)
    {
        float screenWidth = Screen.width;
        return (screenPosition.x < screenWidth * SCREEN_DIVISION_RATIO) ? Joystick.Left : Joystick.Right;
    }

    private Vector2 CalculateDirection(Vector2 initialTouchPosition, Vector2 currentTouchPosition)
    {
        Camera uiCamera = CameraManager.Instance.UICamera.Camera;
        Vector2 worldInitialPos = uiCamera.ScreenToWorldPoint(initialTouchPosition);
        Vector2 worldCurrentPos = uiCamera.ScreenToWorldPoint(currentTouchPosition);
        Vector2 direction = worldCurrentPos - worldInitialPos;
        direction = Vector2.ClampMagnitude(direction, MAX_MAGNITUDE);
        
        return direction;
    }
}
