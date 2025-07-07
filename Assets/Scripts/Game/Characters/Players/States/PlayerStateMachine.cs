using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState CurrentStateType { get; private set; }

    private PlayerStateBase _currentState;
    private Player _player;

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
        _player = GetComponent<Player>();
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
        if (CurrentStateType == newState && _currentState != null)
        {
            return;
        }

        if (_currentState != null)
        {
            _currentState.Exit();
        }

        CurrentStateType = newState;

        _currentState = GetStateInstance(newState);
        if (_currentState != null)
        {
            _currentState.Enter();
        }
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
        if (_currentState == null)
        {
            return;
        }

        _currentState.HandleInput(input);
        PlayerState? nextState = _currentState.CheckTransitions(input);

        if (nextState.HasValue && _currentState.PlayerState != nextState)
        {
            ChangeState(nextState.Value);
        }
    }

    private void Update()
    {
        var input = _player.InputHandler.GetInputData();
        HandleInput(input);

        if (_currentState == null)
        {
            return;
        }

        
        _currentState.Update();
    }

    private void FixedUpdate()
    {
        if (_currentState == null)
        {
            return;
        }

        _currentState.FixedUpdate();
    }
}