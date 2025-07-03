using UnityEngine;

public interface IPlayerMovement
{
    void Move(Vector3 direction);

    void StopMoving();
}