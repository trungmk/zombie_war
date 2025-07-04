using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    private const string STATE_PARAMETER = "State";
    private const string SHOOT_TRIGGER = "Shoot";
    private const string GRENADE_TRIGGER = "ThrowGrenade";
    private const string MOVEMENT_SPEED_PARAM = "MovementSpeed";
    private const string AIM_X_PARAM = "AimX";
    private const string AIM_Y_PARAM = "AimY";

    private int _stateHash;
    private int _shootHash;
    private int _grenadeHash;
    private int _movementSpeedHash;
    private int _aimXHash;
    private int _aimYHash;

    private void Awake()
    {
        // Cache parameter hashes for performance
        _stateHash = Animator.StringToHash(STATE_PARAMETER);
        _shootHash = Animator.StringToHash(SHOOT_TRIGGER);
        _grenadeHash = Animator.StringToHash(GRENADE_TRIGGER);
        _movementSpeedHash = Animator.StringToHash(MOVEMENT_SPEED_PARAM);
        _aimXHash = Animator.StringToHash(AIM_X_PARAM);
        _aimYHash = Animator.StringToHash(AIM_Y_PARAM);
    }

    public void SetMovement(float speed)
    {
        _animator.SetFloat(_movementSpeedHash, speed);
    }

    public void SetAim(Vector2 aim)
    {
        _animator.SetFloat(_aimXHash, aim.x);
        _animator.SetFloat(_aimYHash, aim.y);
    }

    public void TriggerShoot()
    {
        _animator.SetTrigger(_shootHash);
    }

    public void TriggerThrowWeapon()
    {
        _animator.SetTrigger(_grenadeHash);
    }

    public bool IsInState(string stateName)
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    public float GetCurrentStateNormalizedTime()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}