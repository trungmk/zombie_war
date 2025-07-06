using UnityEngine;

public class JoystickUI : MonoBehaviour, IHandleJoyStickDirection
{
    [SerializeField]
    private float _joystickRadius = 1;

    [SerializeField]
    private Transform _innerCircle = null;

    [SerializeField]
    private Transform _outerCircle = null;

    private JoyStickHandler _joyStickHandler;

    private Joystick _joystick;

    public void Setup(JoyStickHandler joyStickHandler, Joystick joystick)
    {
        _joyStickHandler = joyStickHandler;
        _joystick = joystick;
        _joyStickHandler.RegisterHandleJoyStickDirection(joystick, this);
    }

    private void OnDisable()
    {
        _joyStickHandler.UnregisterHandleJoyStickDirection(_joystick, this);
    }

    public void OnDirectionChanged(Vector2 direction)
    {
        Vector2 innerJoystickLocalPos = direction * _joystickRadius;
        _innerCircle.localPosition = innerJoystickLocalPos;
    }

    public void OnDirectionEnded()
    {
        _innerCircle.localPosition = Vector3.zero;
    }

    public void OnStartClicked(Vector2 direction)
    {
    }
}
