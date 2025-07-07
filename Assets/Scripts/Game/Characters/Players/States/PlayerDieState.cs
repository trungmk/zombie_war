using UnityEngine;

public class PlayerDieState : PlayerStateBase
{
    public PlayerDieState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) 
    {
        PlayerState = PlayerState.Die;
    }

    public override void Enter()
    {
        player.Movement.StopMovement();
        player.Shooting.StopShooting();
        //player.AnimationController.SetState(PlayerState.Die);
    }

    public override void HandleInput(PlayerInputData input) { }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        return null;
    }
}