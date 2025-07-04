using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerInputHandler InputHandler;

    public PlayerStateMachine StateMachine;

    public PlayerAnimationController AnimationController;

    public MovementComponent Movement;

    public ShootingComponent Shooting;

    private void Start()
    {
        StateMachine.ChangeState(PlayerState.Idle);
    }

    private void Update()
    {
        var input = InputHandler.GetInputData();
        StateMachine.HandleInput(input);
    }
}