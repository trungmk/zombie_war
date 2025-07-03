using UnityEngine;

public class PlayerDieState : PlayerStateBase
{
    public PlayerDieState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        player.Movement.StopMoving();
        player.AnimationController.SetState(PlayerState.Die);
    }

    public override void HandleInput(PlayerInputData input) { }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        return null;
    }
}