using UnityEngine;

public class PlayerMoveAndThrowGrenadeState : PlayerStateBase
{
    private float _grenadeTimer;
    private float _grenadeDuration = 1f;
    private bool _grenadeThrown = false;
    private Vector3 _movementDirection;

    public PlayerMoveAndThrowGrenadeState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        PlayerState = PlayerState.MoveAndGrenade;
    }

    public override void Enter()
    {
        Debug.Log("Entering Move and Throw Grenade State");
        _grenadeTimer = 0f;
        _grenadeThrown = false;

        player.Shooting.StopShooting();

        player.AnimationController.SetIsFire(false);
        player.AnimationController.TriggerThrowGrenade();
        player.InputHandler.SetGrenade(false);
        player.InputHandler.ResetGrenadeInput();
    }

    public override void Update()
    {
        _grenadeTimer += Time.deltaTime;
        if (!_grenadeThrown && _grenadeTimer >= 0.3f)
        {
            ThrowGrenadeWithCurrentAim();
            _grenadeThrown = true;
        }
    }

    public override void HandleInput(PlayerInputData input)
    {
        _movementDirection = new Vector3(input.MovementInput.x, 0, input.MovementInput.y);
    }

    private void ThrowGrenadeWithCurrentAim()
    {
        if (player.Grenade.CanThrowGrenade())
        {
            Vector3 throwDirection = player.transform.forward;
            Vector2 throwDirection2D = new Vector2(throwDirection.x, throwDirection.z);

            player.Grenade.ThrowGrenade(throwDirection2D);
        }
    }

    public override void FixedUpdate()
    {
        player.Movement.MoveAndRotate(_movementDirection);

        Vector3 animationDirection = _movementDirection * 4f;
        player.AnimationController.SetMovement(animationDirection);
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (_grenadeTimer >= _grenadeDuration)
        {
            if (input.IsMoving && input.ShootTriggered)
                return PlayerState.MoveAndShoot;

            else if (input.IsMoving)
                return PlayerState.Move;

            else if (input.ShootTriggered)
                return PlayerState.Shoot;

            else
                return PlayerState.Idle;
        }

        return null;
    }

    public override void Exit()
    {
        Debug.Log("Exiting Move and Throw Grenade State");
    }
}