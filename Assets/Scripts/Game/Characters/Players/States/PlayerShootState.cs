using UnityEngine;

public class PlayerShootState : PlayerStateBase
{
    private Vector3 _direction;

    public PlayerShootState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entering Shoot State");
        if (!player.Shooting.IsAutoShooting())
        {
            player.Shooting.StartAutoShooting();
        }

        player.AnimationController.SetIsFire();
        player.AnimationController.SetWeaponType((int) WeaponManager.Instance.CurrentProjectileWeapon.ProjectileWeaponType);
    }

    public override void HandleInput(PlayerInputData input)
    {
        _direction = new Vector3(input.AimingInput.x, 0, input.AimingInput.y);

        if (input.IsMoving)
        {
            stateMachine.ChangeState(PlayerState.MoveAndShoot);
        }
        else if (input.GrenadeTriggered)
        {
            stateMachine.ChangeState(PlayerState.ThrowGrenade);
        }
    }

    public override void FixedUpdate()
    {
        player.Shooting.AutoShoot(_direction);
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (input.IsMoving && input.ShootTriggered)
        {
            return PlayerState.MoveAndShoot;
        }
        else if (input.IsMoving)
        {
            return PlayerState.Move;
        }
        else if (!input.ShootTriggered) 
        {
            return PlayerState.Idle;
        }

        return null;
    }

    public override void Exit()
    {
        player.AnimationController.SetWeaponType(0);
        //player.AnimationController.SetIsFire();

        player.Shooting.StopShooting();
        Debug.Log("Exit Shoot State");
    }
}