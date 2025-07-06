using UnityEngine;

public class PlayerMoveAndThrowGrenadeState : PlayerStateBase
{
    private float _grenadeTimer;
    private float _grenadeDuration = 0.5f;

    public PlayerMoveAndThrowGrenadeState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        _grenadeTimer = 0f;
        player.Shooting.StopShooting();
    }

    public override void Update()
    {
        _grenadeTimer += Time.deltaTime;
    }

    public override void HandleInput(PlayerInputData input)
    {
        player.Movement.MoveAndRotate(new Vector3(input.AimingInput.x, 0, input.AimingInput.y));
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (_grenadeTimer >= _grenadeDuration)
        {
            if (input.IsMoving)
                return PlayerState.Move;

            else
                return PlayerState.Idle;
        }

        return null;
    }
}