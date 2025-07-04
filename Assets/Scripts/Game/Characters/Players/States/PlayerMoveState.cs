using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    private Vector3 _direction;

    public PlayerMoveState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void HandleInput(PlayerInputData input)
    {
        _direction = new Vector3(input.MovementInput.x, 0, input.MovementInput.y);
    }

    public override void FixedUpdate()
    {
        player.Movement.Move(_direction);

        float speed = _direction.magnitude * 4f;
        player.AnimationController.SetMovement(speed);
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (!input.IsMoving)
            return PlayerState.Idle;
        
        if (input.IsAiming && input.ShootTriggered && player.Shooting.CanShoot())
            return PlayerState.MoveAndShoot;

        return null;
    }
}