#if UNITY_EDITOR
using UnityEngine;

public class EditorInputTester : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField]
    private bool _enableEditorInput = true;

    [Header("Debug")]
    [SerializeField]
    private bool _showDebugInfo = true;

    private PlayerInputHandler _playerInputHandler;
    private Player _player;
    private MovementComponent _movementComponent;

    private void Awake()
    {
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _player = GetComponent<Player>();
        _movementComponent = GetComponent<MovementComponent>();

        Debug.Log($"PlayerInputHandler: {_playerInputHandler != null}");
        Debug.Log($"Player: {_player != null}");
        Debug.Log($"MovementComponent: {_movementComponent != null}");
    }

    private void Start()
    {
        if (_player != null && _player.StateMachine != null)
        {
            _player.StateMachine.ChangeState(PlayerState.Idle);
            Debug.Log("Player state set to Idle");
        }
    }

    private void Update()
    {
        if (!_enableEditorInput)
            return;

        HandleDirectMovement();
    }

    private void HandleDirectMovement()
    {
        Vector2 inputVector = Vector2.zero;

        // Get input from axes (works in simulator)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            inputVector = new Vector2(horizontal, vertical);
        }

        // Fallback to keys
        if (inputVector == Vector2.zero)
        {
            if (Input.GetKey(KeyCode.W)) inputVector.y += 1f;
            if (Input.GetKey(KeyCode.S)) inputVector.y -= 1f;
            if (Input.GetKey(KeyCode.A)) inputVector.x -= 1f;
            if (Input.GetKey(KeyCode.D)) inputVector.x += 1f;
        }

        if (inputVector.magnitude > 1f)
            inputVector.Normalize();

        // Method 1: Use PlayerInputHandler
        if (_playerInputHandler != null)
        {
            _playerInputHandler.SetMovement(inputVector);
        }

        // Method 2: Direct MovementComponent (bypass state machine)
        if (Input.GetKey(KeyCode.LeftShift) && _movementComponent != null && inputVector != Vector2.zero)
        {
            Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
            _movementComponent.Move(moveDirection);
            Debug.Log($"Direct move: {moveDirection}");
        }

        // Method 3: Force player into move state
        if (Input.GetKey(KeyCode.Space) && _player != null && inputVector != Vector2.zero)
        {
            _player.StateMachine?.ChangeState(PlayerState.Move);
            Debug.Log("Forced Move state");
        }

        if (_showDebugInfo && inputVector != Vector2.zero)
        {
            Debug.Log($"Input: {inputVector}");

            if (_playerInputHandler != null)
            {
                Debug.Log($"Handler Input: {_playerInputHandler.MovementInput}, IsMoving: {_playerInputHandler.IsMoving}");
            }

            if (_movementComponent != null)
            {
                Debug.Log($"Movement IsMoving: {_movementComponent.IsMoving}, Velocity: {_movementComponent.CurrentVelocity}");
            }
        }
    }

    [ContextMenu("Test Direct Movement")]
    public void TestDirectMovement()
    {
        if (_movementComponent != null)
        {
            _movementComponent.Move(Vector3.forward);
            Debug.Log("Direct movement test executed");
        }
    }

    [ContextMenu("Set Player to Move State")]
    public void SetPlayerToMoveState()
    {
        if (_player?.StateMachine != null)
        {
            _player.StateMachine.ChangeState(PlayerState.Move);
            Debug.Log("Player state changed to Move");
        }
    }

    [ContextMenu("Debug Player State")]
    public void DebugPlayerState()
    {
        if (_player?.StateMachine != null)
        {
            Debug.Log($"Current State: {_player.StateMachine.CurrentStateType}");
        }
    }
}
#endif