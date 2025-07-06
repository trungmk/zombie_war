using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entering Idle State");
        player.Movement.StopMovement();
        //player.Shooting.StopShooting();
        player.AnimationController.SetMovement(0);
    }

    public override void HandleInput(PlayerInputData input)
    {
        if (input.ShootTriggered)
        {
            stateMachine.ChangeState(PlayerState.Shoot);
        }
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (input.IsMoving && input.ShootTriggered)
        {
            return PlayerState.MoveAndShoot;
        }

        if (input.ShootTriggered)
        {
            return PlayerState.Shoot;
        }

        if (input.IsMoving)
        {
            return PlayerState.Move;
        }

        if (input.GrenadeTriggered)
        {
            return PlayerState.ThrowGrenade;
        }

        return null;
    }
}