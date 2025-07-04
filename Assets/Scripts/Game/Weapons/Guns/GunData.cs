using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Game/Weapon Data")]
public class GunData : ScriptableObject
{
    public string WeaponName;
    public float Damage = 25f;
    public float FireRate = 0.15f;
    public float BulletSpeed = 20f;
    public float Range = 50f;

    public GameObject BulletPrefab;
    public ParticleSystem HitEffect;
    public AudioClip ShootSound;
    public AudioClip ReloadSound;
}