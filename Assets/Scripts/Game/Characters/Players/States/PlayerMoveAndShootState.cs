using UnityEngine;

public class PlayerMoveAndShootState : PlayerStateBase
{
    public PlayerMoveAndShootState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        //Vector3 direction = new Vector3(player.InputHandler.AimInput.x, 0, player.InputHandler.AimInput.y);
        //player.Shooting.Shoot(direction);
        //player.AnimationController.SetIsFire();
        //player.AnimationController.SetWeaponType((int)WeaponManager.Instance.CurrentProjectileWeapon.WeaponType);

        if (!player.Shooting.IsAutoShooting())
        {
            player.Shooting.StartAutoShooting();
        }

        player.AnimationController.SetIsFire();
        player.AnimationController.SetWeaponType((int)WeaponManager.Instance.CurrentProjectileWeapon.ProjectileWeaponType);
    }

    public override void HandleInput(PlayerInputData input)
    {
        Vector3 direction = new Vector3(input.MovementInput.x, 0, input.MovementInput.y);
        player.Movement.Move(direction);
        player.Shooting.AutoShoot(direction);
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
        player.Shooting.StopShooting();
        player.AnimationController.SetWeaponType(0);
    }
}