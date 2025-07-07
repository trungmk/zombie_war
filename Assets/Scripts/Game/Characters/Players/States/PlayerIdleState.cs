using System;
using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        PlayerState = PlayerState.Idle;
    }

    public override void Enter()
    {
        player.Movement.StopMovement();
        player.AnimationController.SetMovement(0);

        WeaponManager.Instance.OnUseGrenade += HandleGrenadeInput;
    }

    private void HandleGrenadeInput(GrenadeWeapon weapon)
    {
        
    }

    public override void HandleInput(PlayerInputData input)
    {
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (input.GrenadeTriggered && player.Grenade.CanThrowGrenade())
            return PlayerState.ThrowGrenade;

        if (input.ShootTriggered)
            return PlayerState.Shoot;

        if (input.IsMoving)
            return PlayerState.Move;

        return null;
    }
}