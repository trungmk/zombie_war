using UnityEngine;

[CreateAssetMenu(fileName = "New Grenade", menuName = "Game/Grenade Data")]
public class GrenadeData : ScriptableObject
{
    [Header("Basic Settings")]
    public string GrenadeName = "Frag Grenade";
    public float Damage = 80f;
    public float ExplosionRadius = 5f;
    public float ExplosionForce = 700f;
    public float FuseTime = 3f;

    public float ThrowForce = 15f;
    public float MaxThrowDistance = 20f;
    public AnimationCurve ThrowCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Effects")]
    public GameObject ExplosionEffect;
    public AudioClip ExplosionSound;
    public AudioClip PinPullSound;
}