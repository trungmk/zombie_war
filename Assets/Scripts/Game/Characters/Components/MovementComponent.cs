using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementComponent : MonoBehaviour
{
    [SerializeField] 
    private float _moveSpeed = 5f;

    [SerializeField] 
    private float _rotationSpeed = 720f;

    private Rigidbody _rigidbody;

    public bool IsMoving { get; private set; }
    public Vector3 CurrentVelocity => _rigidbody.linearVelocity;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 direction)
    {
        Vector3 velocity = direction.normalized * _moveSpeed;
        velocity.y = _rigidbody.linearVelocity.y; 
        _rigidbody.linearVelocity = velocity;
        IsMoving = velocity.magnitude > 0.01f;
    }

    public void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;
        Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
    }

    public void StopMoving()
    {
        _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
        IsMoving = false;
    }
}