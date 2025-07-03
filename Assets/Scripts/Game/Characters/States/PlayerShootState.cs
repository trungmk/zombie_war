using UnityEngine;

public class PlayerShootState : PlayerStateBase
{
    private float _shootTimer;
    private float _shootDuration = 0.2f;

    public PlayerShootState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

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
        if (input.isMoving)
        {
            stateMachine.ChangeState(PlayerState.MoveAndShoot);
        }    
        else if (input.grenadeTriggered)
        {
            stateMachine.ChangeState(PlayerState.ThrowGrenade);
        }    
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (_shootTimer >= _shootDuration)
        {
            if (input.isMoving)
                return PlayerState.Move;
            else
                return PlayerState.Idle;
        }

        if (input.isMoving)
            return PlayerState.MoveAndShoot;

        return null;
    }
}