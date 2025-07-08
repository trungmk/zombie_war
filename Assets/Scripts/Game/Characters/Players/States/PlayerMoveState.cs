using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    private Vector3 _direction;

    public PlayerMoveState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) 
    {
        PlayerState = PlayerState.Move;
        _direction = Vector3.zero;
    }

    public override void HandleInput(PlayerInputData input)
    {
        _direction = new Vector3(input.MovementInput.x, 0, input.MovementInput.y);
    }

    public override void FixedUpdate()
    {
        player.Movement.MoveAndRotate(_direction);
        Vector3 animationDirection = new Vector3(0, 0, _direction.magnitude * 4f);
        player.AnimationController.SetMovement(animationDirection);
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (input.GrenadeTriggered && player.Grenade.CanThrowGrenade())
            return PlayerState.MoveAndGrenade;

        if (input.ShootTriggered)
            return PlayerState.MoveAndShoot;

        if (!input.IsMoving)
            return PlayerState.Idle;

        return null;
    }
}