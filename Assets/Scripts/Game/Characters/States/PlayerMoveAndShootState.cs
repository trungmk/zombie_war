using UnityEngine;

public class PlayerMoveAndShootState : PlayerStateBase
{
    public PlayerMoveAndShootState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void HandleInput(PlayerInputData input)
    {
        player.Movement.Move(new Vector3(input.movementInput.x, 0, input.movementInput.y));
        player.Shooting.AutoShoot(input.aimInput);
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (!input.isMoving && !input.isAiming)
            return PlayerState.Idle;
        
        if (!input.isMoving)
            return PlayerState.Shoot;
        
        if (!input.isAiming)
            return PlayerState.Move;

        return null;
    }
}