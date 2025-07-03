using UnityEngine;

public class RectUtil
{
    public static Joystick GetJoystickFromTouchPosition(Vector2 touchPosition, float screenDivisionRatio)
    {
        if (touchPosition.x < Screen.width * screenDivisionRatio)
        {
            return Joystick.Left;
        }
        else
        {
            return Joystick.Right;
        }
    }
}