using UnityEngine;

public interface IHandleJoyStickDirection
{
    void OnDirectionChanged(Vector2 direction);

    void OnDirectionEnded();
}