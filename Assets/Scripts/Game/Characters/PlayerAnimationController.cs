using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animation Parameters")]
    [SerializeField] private string _stateParameter = "State";
    [SerializeField] private string _moveXParameter = "MoveX";
    [SerializeField] private string _moveYParameter = "MoveY";
    [SerializeField] private string _aimXParameter = "AimX";
    [SerializeField] private string _aimYParameter = "AimY";
    [SerializeField] private string _shootTrigger = "Shoot";
    [SerializeField] private string _grenadeTrigger = "ThrowGrenade";

    private Animator _animator;
    private int _stateHash;
    private int _moveXHash;
    private int _moveYHash;
    private int _aimXHash;
    private int _aimYHash;
    private int _shootHash;
    private int _grenadeHash;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        // Cache parameter hashes for performance
        _stateHash = Animator.StringToHash(_stateParameter);
        _moveXHash = Animator.StringToHash(_moveXParameter);
        _moveYHash = Animator.StringToHash(_moveYParameter);
        _aimXHash = Animator.StringToHash(_aimXParameter);
        _aimYHash = Animator.StringToHash(_aimYParameter);
        _shootHash = Animator.StringToHash(_shootTrigger);
        _grenadeHash = Animator.StringToHash(_grenadeTrigger);
    }

    public void SetState(PlayerState state)
    {
        _animator.SetInteger(_stateHash, (int)state);
    }

    public void SetMovement(Vector2 movement)
    {
        _animator.SetFloat(_moveXHash, movement.x);
        _animator.SetFloat(_moveYHash, movement.y);
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