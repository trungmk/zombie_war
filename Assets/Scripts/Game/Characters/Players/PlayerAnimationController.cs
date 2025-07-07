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
    private const string MOVEMENT_SPEED_PARAM = "MovementSpeed";
    private const string WEAPONTYPE_PARAM = "WeaponType";
    private const string IS_FIRING_PARAM = "IsFire";
    private const string CHANGE_WEAPON = "ChangeWeapon";

    private int _grenadeHash;
    private int _movementSpeedHash;
    private int _weaponTypeHash;
    private int _fireHash;
    private int _changeWeaponHash;

    public Action OnStartThrowGrenade { get; set; }

    private void Awake()
    {
        // Cache parameter hashes for performance
        _grenadeHash = Animator.StringToHash(GRENADE_TRIGGER);
        _movementSpeedHash = Animator.StringToHash(MOVEMENT_SPEED_PARAM);
        _weaponTypeHash = Animator.StringToHash(WEAPONTYPE_PARAM);
        _fireHash = Animator.StringToHash(IS_FIRING_PARAM);
        _changeWeaponHash = Animator.StringToHash(CHANGE_WEAPON);

        StartRig();
    }

    public void SetMovement(float speed)
    {
        _animator.SetFloat(_movementSpeedHash, speed);
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
