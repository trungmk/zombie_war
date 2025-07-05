using UnityEngine;

public class Player : MonoBehaviour
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

    private void Start()
    {
        Health.Setup(PlayerData.MaxHealth);
        Shooting.Setup(WeaponTransform);
        Grenade.Setup();
        StateMachine.ChangeState(PlayerState.Idle);
    }
}