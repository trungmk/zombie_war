using UnityEngine;

public interface IHandleJoyStickDirection
{
    void OnDirectionChanged(Vector2 direction);

    void OnDirectionEnded();

    void OnStartClicked(Vector2 direction);
}