using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Player : BaseCharacter, ITakeDamage
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
        Shooting.Setup(this);
        Grenade.Setup(this);

        Health.Setup(PlayerData.MaxHealth);
        Health.OnHealthChanged = Handle_OnHealthChanged;
        Health.OnDied = Handle_OnDied;

        StateMachine.ChangeState(PlayerState.Idle);
    }

    private void Handle_OnDied()
    {
        Debug.Log("Player Died");
    }

    private void Handle_OnHealthChanged(int currentHealth, int maxHealth)
    {
        
    }

    public void TakeDamage(int damageAmount)
    {
        Health.TakeDamage(damageAmount);
    }
}