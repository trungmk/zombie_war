using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState CurrentStateType { get; private set; }

    private PlayerStateBase _currentState;
    private PlayerController _player;

    // States
    private PlayerIdleState _idleState;
    private PlayerMoveState _moveState;
    private PlayerShootState _shootState;
    private PlayerThrowGrenadeState _throwGrenadeState;
    private PlayerMoveAndShootState _moveAndShootState;
    private PlayerMoveAndThrowGrenadeState _moveAndThrowGrenadeState;
    private PlayerDieState _dieState;

    private void Awake()
    {
        _player = GetComponent<PlayerController>();
        _idleState = new PlayerIdleState(_player, this);
        _moveState = new PlayerMoveState(_player, this);
        _shootState = new PlayerShootState(_player, this);
        _throwGrenadeState = new PlayerThrowGrenadeState(_player, this);
        _moveAndShootState = new PlayerMoveAndShootState(_player, this);
        _moveAndThrowGrenadeState = new PlayerMoveAndThrowGrenadeState(_player, this);
        _dieState = new PlayerDieState(_player, this);
    }

    public void ChangeState(PlayerState newState)
    {
        if (CurrentStateType == newState) return;
        _currentState?.Exit();
        CurrentStateType = newState;
        _currentState = GetStateInstance(newState);
        _currentState?.Enter();
        _player.AnimationController?.SetState(newState);
    }

    private PlayerStateBase GetStateInstance(PlayerState state)
    {
        return state switch
        {
            PlayerState.Idle => _idleState,
            PlayerState.Move => _moveState,
            PlayerState.Shoot => _shootState,
            PlayerState.ThrowGrenade => _throwGrenadeState,
            PlayerState.MoveAndShoot => _moveAndShootState,
            PlayerState.MoveAndGrenade => _moveAndThrowGrenadeState,
            PlayerState.Die => _dieState,
            _ => _idleState
        };
    }

    public void HandleInput(PlayerInputData input)
    {
        _currentState?.HandleInput(input);
        _currentState?.Update();
        
        var nextState = _currentState?.CheckTransitions(input);
        if (nextState.HasValue)
        {
            ChangeState(nextState.Value);
        }
    }

    private void FixedUpdate()
    {
        _currentState?.FixedUpdate();
    }
}