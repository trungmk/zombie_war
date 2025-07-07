using UnityEngine;

public class PlayerThrowGrenadeState : PlayerStateBase
{
    private float _grenadeTimer;
    private float _grenadeThrowDelay = 0.5f;
    private bool _grenadeThrown = false;

    public PlayerThrowGrenadeState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        PlayerState = PlayerState.ThrowGrenade;
    }

    public override void Enter()
    {
        Debug.Log("Entering Throw Grenade State");
        _grenadeTimer = 0f;
        _grenadeThrown = false;

        player.Shooting.StopShooting();
        player.AnimationController.TriggerThrowGrenade();

        if (player.AnimationController.OnStartThrowGrenade != null)
        {
            player.AnimationController.OnStartThrowGrenade += ThrowGrenadeEvent;
        }
    }

    public override void Update()
    {
        _grenadeTimer += Time.deltaTime;

        if (!_grenadeThrown && _grenadeTimer >= _grenadeThrowDelay)
        {
            _grenadeTimer = 0f;
            ThrowGrenadeNow();
        }
    }

    public override void HandleInput(PlayerInputData input)
    {
        
    }

    private void ThrowGrenadeEvent()
    {
        if (!_grenadeThrown)
        {
            ThrowGrenadeNow();
        }
    }

    private void ThrowGrenadeNow()
    {
        if (_grenadeThrown)
        {
            return;
        }

        _grenadeThrown = true;
        player.InputHandler.SetGrenade(false);

        if (player.Grenade.CanThrowGrenade())
        {
            Vector3 throwDirection = player.transform.forward;
            Vector2 throwDirection2D = new Vector2(throwDirection.x, throwDirection.z);

            player.Grenade.ThrowGrenade(throwDirection2D);
        }
    }

    public override PlayerState? CheckTransitions(PlayerInputData input)
    {
        if (_grenadeThrown && _grenadeTimer >= 0.8f)
        {
            if (input.IsMoving && input.ShootTriggered)
            {
                return PlayerState.MoveAndShoot;
            }    
            else if (input.IsMoving)
            {
                return PlayerState.Move;
            }
            else if (input.ShootTriggered)
            {
                return PlayerState.Shoot;
            }
            else
            {
                return PlayerState.Idle;
            }
        }

        if (input.IsMoving && _grenadeTimer > 0.3f)
        {
            return PlayerState.MoveAndGrenade;
        }

        return null;
    }

    public override void Exit()
    {
        Debug.Log("Exiting Throw Grenade State");

        if (player.AnimationController.OnStartThrowGrenade != null)
        {
            player.AnimationController.OnStartThrowGrenade -= ThrowGrenadeEvent;
        }

        _grenadeTimer = 0f;
        _grenadeThrown = false;
        player.InputHandler.SetGrenade(false);
    }
}