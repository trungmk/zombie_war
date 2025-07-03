using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public PlayerInputHandler InputHandler;
    public PlayerStateMachine StateMachine;
    public PlayerAnimationController AnimationController;
    public MovementComponent Movement;
    public ShootingComponent Shooting;

    private void Awake()
    {
        InputHandler = GetComponent<PlayerInputHandler>();
        StateMachine = GetComponent<PlayerStateMachine>();
        AnimationController = GetComponent<PlayerAnimationController>();
        Movement = GetComponent<MovementComponent>();
        Shooting = GetComponent<ShootingComponent>();
    }

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