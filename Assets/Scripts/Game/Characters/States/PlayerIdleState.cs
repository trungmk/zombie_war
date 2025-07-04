using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entering Idle State");
        player.Movement.StopMovement();
        player.AnimationController.SetMovement(0);
    }

    public override void HandleInput(PlayerInputData input)
    {
        
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (input.IsAiming && input.ShootTriggered && player.Shooting.CanShoot())
            return PlayerState.Shoot;

        if (input.IsMoving)
            return PlayerState.Move;

        return null;
    }
}