using UnityEngine;

public struct InputData
{
    public int TouchId;

    public Vector2 TouchPosition;

    public InputData(int touchId, Vector2 touchPosition)
    {
        TouchId = touchId;
        TouchPosition = touchPosition;
    }
}

public interface IInputHandler 
{
    void OnUserPress(InputData inputData);

    void OnUserDrag(InputData inputData);

    void OnUserRelease(InputData inputData);
}
