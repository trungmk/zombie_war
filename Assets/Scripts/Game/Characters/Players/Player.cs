using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Player : BaseCharacter
{
    public PlayerInputHandler InputHandler;

    public PlayerStateMachine StateMachine;

    public PlayerAnimationController AnimationController;

    public MovementComponent Movement;

    public ShootingComponent Shooting;

    public HealthComponent Health;

    public GrenadeComponent Grenade;

    public Transform WeaponTransform;

    public PlayerData PlayerData;

    public Rig Rig;

    private void Start()
    {
        Movement.Setup(PlayerData.MoveSpeed, PlayerData.RotationSpeed);
        Health.Setup(PlayerData.MaxHealth);
        Shooting.Setup(this);
        Grenade.Setup();
        StateMachine.ChangeState(PlayerState.Idle);
    }
}