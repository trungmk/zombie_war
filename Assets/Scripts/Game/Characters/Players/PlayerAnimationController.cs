using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private Rig _rig;

    private const string GRENADE_TRIGGER = "ThrowGrenade";
    private const string MOVEMENT_X_PARAM = "MovementX";
    private const string MOVEMENT_Z_PARAM = "MovementZ";
    private const string WEAPONTYPE_PARAM = "WeaponType";
    private const string IS_FIRING_PARAM = "IsFire";
    private const string CHANGE_WEAPON = "ChangeWeapon";
    private const string IS_MOVING_PARAM = "IsMoving";

    private int _grenadeHash;
    private int _movementXHash;
    private int _movementZHash;
    private int _weaponTypeHash;
    private int _fireHash;
    private int _changeWeaponHash;
    private int _isMovingHash;

    public Action OnStartThrowGrenade { get; set; }

    private void Awake()
    {
        // Cache parameter hashes for performance
        _grenadeHash = Animator.StringToHash(GRENADE_TRIGGER);
        _movementXHash = Animator.StringToHash(MOVEMENT_X_PARAM);
        _movementZHash = Animator.StringToHash(MOVEMENT_Z_PARAM);
        _weaponTypeHash = Animator.StringToHash(WEAPONTYPE_PARAM);
        _fireHash = Animator.StringToHash(IS_FIRING_PARAM);
        _changeWeaponHash = Animator.StringToHash(CHANGE_WEAPON);
        _isMovingHash = Animator.StringToHash(IS_MOVING_PARAM);

        StartRig();
    }

    public void SetMovement(float x, float y)
    {
        _animator.SetFloat(_movementXHash, x);
        _animator.SetFloat(_movementZHash, y);
    }

    public void SetMovement(Vector3 direction)
    {
        _animator.SetFloat(_movementXHash, direction.x);
        _animator.SetFloat(_movementZHash, direction.z); 
    }

    public void SetIsMoving(bool isMoving)
    {
        _animator.SetBool(_isMovingHash, isMoving);
    }

    public void SetIsFire(bool isFire)
    {
        _animator.SetBool(_fireHash, isFire);
    }

    public void SetWeaponType(int weaponType)
    {
        _animator.SetInteger(_weaponTypeHash, weaponType);
    }

    public void TriggerThrowGrenade()
    {
        _animator.SetTrigger(_grenadeHash);
    }

    public float GetCurrentStateNormalizedTime()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void TriggerChangeWeapon()
    {
        _animator.SetTrigger(_changeWeaponHash);
    }

    public void StartThrowGrenadeEvent()
    {
        Debug.Log("AnimationController: Starting throw grenade event");
        if (OnStartThrowGrenade != null)
        {
            OnStartThrowGrenade();
        }
    }

    public void StopRig()
    {
        if (_rig != null)
        {
            _rig.weight = 0f;
        }
    }

    public void StartRig()
    {
        if (_rig != null)
        {
            _rig.weight = 1f;
        }
    }
}