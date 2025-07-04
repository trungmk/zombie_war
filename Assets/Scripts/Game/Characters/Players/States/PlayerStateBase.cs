using UnityEngine;

public abstract class PlayerStateBase
{
    protected PlayerController player;
    protected PlayerStateMachine stateMachine;

    public PlayerStateBase(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void HandleInput(PlayerInputData input) { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public abstract PlayerState? CheckTransitions(PlayerInputData input);
}