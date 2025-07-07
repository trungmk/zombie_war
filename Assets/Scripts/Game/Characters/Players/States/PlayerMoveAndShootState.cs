using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerMoveAndShootState : PlayerStateBase
{
    private Vector3 _movementDirection;

    private Vector3 _aimingDirection;

    public PlayerMoveAndShootState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        PlayerState = PlayerState.MoveAndShoot;
        _movementDirection = Vector3.zero;
        _aimingDirection = Vector3.zero;
    }

    public override void Enter()
    {
        if (!player.Shooting.IsAutoShooting())
        {
            player.Shooting.StartAutoShooting();
        }

        player.AnimationController.SetIsFire(true);
        player.AnimationController.SetWeaponType((int) WeaponManager.Instance.CurrentProjectileWeapon.ProjectileWeaponType);
    }

    public override void HandleInput(PlayerInputData input)
    {
        _movementDirection = new Vector3(input.MovementInput.x, 0, input.MovementInput.y);
        _aimingDirection = new Vector3(input.AimingInput.x, 0, input.AimingInput.y);
    }

    public override void FixedUpdate()
    {
        if (_movementDirection.magnitude > 0.01f)
        {
            player.Movement.Move(_movementDirection);
            float speed = _movementDirection.magnitude * 4f;
            player.AnimationController.SetMovement(speed);
        }
        else
        {
            player.Movement.Move(Vector3.zero);
            player.AnimationController.SetMovement(0f);
        }

        if (_aimingDirection.magnitude > 0.01f)
        {
            player.Movement.Rotate(_aimingDirection);
            player.Shooting.AutoShoot(_aimingDirection);
        }
        else
        {
            _aimingDirection = player.transform.forward;
            player.Shooting.AutoShoot(_aimingDirection);
        }
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (input.GrenadeTriggered && player.Grenade.CanThrowGrenade())
        {
            if (input.IsMoving)
                return PlayerState.MoveAndGrenade;
            else
                return PlayerState.ThrowGrenade;
        }

        if (!input.IsMoving && !input.ShootTriggered)
            return PlayerState.Idle;

        if (!input.IsMoving)
            return PlayerState.Shoot;

        if (!input.ShootTriggered)
            return PlayerState.Move;

        return null;
    }

    public override void Exit()
    {
        player.AnimationController.SetIsFire(false);
        player.Shooting.StopShooting();
        player.AnimationController.SetWeaponType(0);
    }
}