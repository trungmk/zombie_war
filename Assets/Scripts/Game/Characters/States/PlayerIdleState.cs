using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.Movement.StopMoving();
    }

    public override void HandleInput(PlayerInputData input)
    {
        
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (input.isAiming && input.shootTriggered && player.Shooting.CanShoot())
            return PlayerState.Shoot;

        if (input.isMoving)
            return PlayerState.Move;

        return null;
    }
}