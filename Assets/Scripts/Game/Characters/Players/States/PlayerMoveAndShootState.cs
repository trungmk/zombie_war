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
        player.AnimationController.SetWeaponType((int)WeaponManager.Instance.CurrentProjectileWeapon.ProjectileWeaponType);
    }

    public override void HandleInput(PlayerInputData input)
    {
        if (!input.IsMoving)
        {
            player.Movement.Move(Vector3.zero);
            player.AnimationController.SetMovement(Vector3.zero);
            stateMachine.ChangeState(PlayerState.Shoot);
        }

        _movementDirection = new Vector3(input.MovementInput.x, 0, input.MovementInput.y);
        _aimingDirection = new Vector3(input.AimingInput.x, 0, input.AimingInput.y);
    }

    public override void FixedUpdate()
    {
        if (_movementDirection.magnitude > 0.01f)
        {
            player.Movement.Move(_movementDirection);
            Vector3 animationDirection = CalculateAnimationDirection();
            player.AnimationController.SetMovement(animationDirection);
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

    private Vector3 CalculateAnimationDirection()
    {
        Vector3 localMovement = player.transform.InverseTransformDirection(_movementDirection);

        if (_aimingDirection.magnitude > 0.01f)
        {
            Vector3 localAiming = player.transform.InverseTransformDirection(_aimingDirection.normalized);

            float forwardComponent = Vector3.Dot(localMovement, localAiming);
            float rightComponent = Vector3.Cross(localMovement, localAiming).y;

            Vector3 adjustedDirection = new Vector3(rightComponent, 0, forwardComponent);
            return adjustedDirection * 4f;
        }

        return localMovement * 4f;
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
        {
            return PlayerState.Idle;
        }
        if (input.IsMoving && !input.ShootTriggered)
        {
            return PlayerState.Move;
        }
        if (!input.IsMoving && input.ShootTriggered)
        {
            return PlayerState.Shoot;
        }

        return null;
    }

    public override void Exit()
    {
        player.AnimationController.SetIsFire(false);
        player.Shooting.StopShooting();
        player.AnimationController.SetWeaponType(0);
    }
}