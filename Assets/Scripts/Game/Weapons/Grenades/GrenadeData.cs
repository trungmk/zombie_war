using UnityEngine;

[CreateAssetMenu(fileName = "New GrenadeWeaponData", menuName = "Game/Grenade Weapon Data")]
public class GrenadeWeaponData : ScriptableObject
{
    public string GrenadeName;

    public float Damage = 80f;

    public float ExplosionRadius = 5f;

    public float ExplosionForce = 700f;

    public float ThrowForce = 15f;

    public float MaxThrowDistance = 20f;

    public float CountDownTime = 2f;

    public AnimationCurve ThrowCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public GameObject ExplosionEffect;

    public AudioClip ExplosionSound;

    public AudioClip PinPullSound;
}