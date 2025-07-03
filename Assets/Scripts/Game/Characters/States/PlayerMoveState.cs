using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    public PlayerMoveState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void HandleInput(PlayerInputData input)
    {
        player.Movement.Move(new Vector3(input.movementInput.x, 0, input.movementInput.y));
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (!input.isMoving)
            return PlayerState.Idle;
        
        if (input.isAiming && input.shootTriggered && player.Shooting.CanShoot())
            return PlayerState.MoveAndShoot;

        return null;
    }
}