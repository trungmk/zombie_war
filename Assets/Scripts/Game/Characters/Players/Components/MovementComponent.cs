using UnityEngine;

public class MovementComponent : BaseComponent
{
    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField]
    private float _directionOffset = 50f; 

    private float _moveSpeed = 5f;
    private float _rotationSpeed = 360f;

    public bool IsMoving { get; private set; }
    public Vector3 CurrentVelocity => _rigidbody.linearVelocity;

    public void Setup(float moveSpeed, float rotationSpeed)
    {
        _moveSpeed = moveSpeed;
        _rotationSpeed = rotationSpeed;
    }

    public void Move(Vector3 direction)
    {
        Vector3 offsetDirection = ApplyDirectionOffset(direction);

        Vector3 velocity = offsetDirection * _moveSpeed;
        velocity.y = _rigidbody.linearVelocity.y;
        _rigidbody.linearVelocity = velocity * Time.fixedDeltaTime;
    }    

    public void MoveAndRotate(Vector3 direction)
    {
        Vector3 offsetDirection = ApplyDirectionOffset(direction);

        Vector3 velocity = offsetDirection * _moveSpeed;
        velocity.y = _rigidbody.linearVelocity.y;
        _rigidbody.linearVelocity = velocity * Time.fixedDeltaTime; 

        IsMoving = direction.magnitude > 0.01f;

        if (IsMoving)
        {
            RotateTowards(offsetDirection);
        }
    }

    public Vector3 ApplyDirectionOffset(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f)
            return direction;

        Quaternion offsetRotation = Quaternion.Euler(0, _directionOffset, 0);
        return offsetRotation * direction;
    }

    public void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f)
        {
            return;
        }

        Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
        float rotationFactor = _rotationSpeed * Time.fixedDeltaTime;
        _rigidbody.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationFactor);
    }

    public void Rotate(Vector3 direction)
    {
        Vector3 offsetDirection = ApplyDirectionOffset(direction);
        RotateTowards(offsetDirection);
    }

    public void StopMovement()
    {
        Debug.Log("Stopping Movement");
        _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
        _rigidbody.angularVelocity = Vector3.zero;
        IsMoving = false;
    }

    public void SetDirectionOffset(float offsetDegrees)
    {
        _directionOffset = offsetDegrees;
    }
}