using UnityEngine;

public class PlayerMoveAndThrowGrenadeState : PlayerStateBase
{
    private float _grenadeTimer;
    private float _grenadeDuration = 0.5f;

    public PlayerMoveAndThrowGrenadeState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        _grenadeTimer = 0f;
    }

    public override void Update()
    {
        _grenadeTimer += Time.deltaTime;
    }

    public override void HandleInput(PlayerInputData input)
    {
        player.Movement.Move(new Vector3(input.movementInput.x, 0, input.movementInput.y));
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (_grenadeTimer >= _grenadeDuration)
        {
            if (input.isMoving)
                return PlayerState.Move;
            else
                return PlayerState.Idle;
        }

        return null;
    }
}