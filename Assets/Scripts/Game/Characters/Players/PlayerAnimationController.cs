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
    private const string IS_FIRING_PARAM = "Fire";
    private const string CHANGE_WEAPON = "ChangeWeapon";

    private int _grenadeHash;
    private int _movementSpeedHash;
    private int _weaponTypeHash;
    private int _fireHash;
    private int _changeWeaponHash;

    private void Awake()
    {
        // Cache parameter hashes for performance
        _grenadeHash = Animator.StringToHash(GRENADE_TRIGGER);
        _movementSpeedHash = Animator.StringToHash(MOVEMENT_SPEED_PARAM);
        _weaponTypeHash = Animator.StringToHash(WEAPONTYPE_PARAM);
        _fireHash = Animator.StringToHash(IS_FIRING_PARAM);
        _changeWeaponHash = Animator.StringToHash(CHANGE_WEAPON);
    }

    public void SetMovement(float speed)
    {
        _animator.SetFloat(_movementSpeedHash, speed);
    }

    public void SetIsFire()
    {
        _animator.SetTrigger(_fireHash);
    }

    public void SetWeaponType(int weaponType)
    {
        _animator.SetInteger(_weaponTypeHash, weaponType);
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

    public void TriggerChangeWeapon()
    {
        _animator.SetTrigger(_changeWeaponHash);
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
