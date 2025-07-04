using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField] 
    private float _moveSpeed = 5f;

    [SerializeField] 
    private float _rotationSpeed = 120f;

    public bool IsMoving { get; private set; }
    public Vector3 CurrentVelocity => _rigidbody.linearVelocity;

    public void Move(Vector3 direction)
    {
        Vector3 velocity = direction * _moveSpeed;
        velocity.y = _rigidbody.linearVelocity.y; 
        _rigidbody.linearVelocity = velocity * Time.fixedDeltaTime;

        IsMoving = direction.magnitude > 0.01f;

        if (IsMoving)
        {
            RotateTowards(direction);
        }
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

    public void StopMovement()
    {
        Debug.Log("Stopping Movement");
        _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
        _rigidbody.angularVelocity = Vector3.zero;
        IsMoving = false;
    }
}