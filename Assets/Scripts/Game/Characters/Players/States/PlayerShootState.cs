using UnityEngine;

public class PlayerShootState : PlayerStateBase
{
    private float _shootTimer;
    private float _shootDuration = 0.2f;

    public PlayerShootState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        _shootTimer = 0f;
        player.Shooting.Shoot(player.InputHandler.AimInput);
        player.AnimationController.TriggerShoot();
    }

    public override void Update()
    {
        _shootTimer += Time.deltaTime;
    }

    public override void HandleInput(PlayerInputData input)
    {
        if (input.IsMoving)
        {
            stateMachine.ChangeState(PlayerState.MoveAndShoot);
        }    
        else if (input.GrenadeTriggered)
        {
            stateMachine.ChangeState(PlayerState.ThrowGrenade);
        }    
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (_shootTimer >= _shootDuration)
        {
            if (input.IsMoving)
                return PlayerState.Move;
            else
                return PlayerState.Idle;
        }

        if (input.IsMoving)
            return PlayerState.MoveAndShoot;

        return null;
    }
}