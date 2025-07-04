using UnityEngine;

public class PlayerMoveAndShootState : PlayerStateBase
{
    public PlayerMoveAndShootState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void HandleInput(PlayerInputData input)
    {
        player.Movement.Move(new Vector3(input.MovementInput.x, 0, input.MovementInput.y));
        player.Shooting.Shoot(input.AimInput);
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (!input.IsMoving && !input.IsAiming)
            return PlayerState.Idle;
        
        if (!input.IsMoving)
            return PlayerState.Shoot;
        
        if (!input.IsAiming)
            return PlayerState.Move;

        return null;
    }
}