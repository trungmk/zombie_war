using Core;
using UnityEngine;

public class InGamePanel : PanelView, ITouchTarget
{
    [SerializeField]
    private RectTransform _rect;

    [SerializeField]
    private JoystickUI _leftJoystick;

    [SerializeField]
    private JoystickUI _righttJoystick;

    [SerializeField]
    private RectTransform _leftJoystickOriginPosition;

    [SerializeField]
    private RectTransform _rightJoystickOriginPosition;

    private JoyStickHandler _joyStickHandler;

    private Camera _uiCamera;

    public bool IsActive => gameObject.activeSelf;

    private Vector2 _cachedLeftJoystickPosition;

    private Vector2 _cachedRightJoystickPosition;

    protected override void OnPanelShowed(params object[] args)
    {
        _joyStickHandler = args[0] as JoyStickHandler;
        if (_joyStickHandler == null)
        {
            Debug.LogError("JoyStickHandler is null. Cannot setup InGamePanel.");
            return;
        }

        _uiCamera = CameraManager.Instance.UICamera.Camera;
        _cachedLeftJoystickPosition = _leftJoystickOriginPosition.position;
        _cachedRightJoystickPosition = _rightJoystickOriginPosition.position;
        _leftJoystick.Setup(_joyStickHandler, Joystick.Left);
        _righttJoystick.Setup(_joyStickHandler, Joystick.Right);
    }

    public void TouchedDown(InputData inputData)
    {
        Joystick joystick = RectUtil.GetJoystickFromTouchPosition(inputData.TouchPosition, 0.5f);
        Vector2 worldPos = _uiCamera.ScreenToWorldPoint(inputData.TouchPosition);
        Vector2 localTouchPos = _rect.InverseTransformPoint(worldPos);

        switch (joystick)
        {
            case Joystick.Left:
                _leftJoystick.transform.localPosition = localTouchPos;

                break;
            case Joystick.Right:
                _righttJoystick.transform.localPosition = localTouchPos;

                break;
            default:
                Debug.LogWarning("Unknown joystick type for touch position: " + inputData.TouchPosition);
                break;
        }
    }

    public void Dragged(InputData inputData)
    {
    }

    public void TouchedUp(InputData inputData)
    {
        _leftJoystick.transform.localPosition = _leftJoystickOriginPosition.localPosition;
        _righttJoystick.transform.localPosition = _rightJoystickOriginPosition.localPosition;
    }

    public void SetActive(bool active)
    {
        
    }
}
