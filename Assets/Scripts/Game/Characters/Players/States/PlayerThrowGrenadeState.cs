using UnityEngine;

public class PlayerThrowGrenadeState : PlayerStateBase
{
    private float _grenadeTimer;
    private float _grenadeDuration = 0.5f;

    public PlayerThrowGrenadeState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        _grenadeTimer = 0f;
        player.AnimationController.TriggerThrowWeapon();
    }

    public override void Update()
    {
        _grenadeTimer += Time.deltaTime;
    }

    public override void HandleInput(PlayerInputData input)
    {
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (_grenadeTimer >= _grenadeDuration)
        {
            if (input.IsMoving)
                return PlayerState.Move;
            else
                return PlayerState.Idle;
        }

        if (input.IsMoving)
            return PlayerState.MoveAndGrenade;

        return null;
    }
}