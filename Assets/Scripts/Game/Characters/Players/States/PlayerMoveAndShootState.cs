using UnityEngine;

public class PlayerMoveAndShootState : PlayerStateBase
{
    public PlayerMoveAndShootState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

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
        Vector3 direction = new Vector3(input.MovementInput.x, 0, input.MovementInput.y);
        player.Movement.Move(direction);

        // Aiming and shooting.
        if (input.AimingInput.magnitude > 0.01f)
        {
            Vector3 aimDirection = new Vector3(input.AimingInput.x, 0, input.AimingInput.y);
            player.Movement.Rotate(aimDirection);
            player.Shooting.AutoShoot(aimDirection);
        }
        else
        {
            Vector3 forwardDirection = player.transform.forward;
            player.Shooting.AutoShoot(forwardDirection);
        }
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
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