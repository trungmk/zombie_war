using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponType WeaponType;

    public abstract void Fire(Vector3 direction);

    public virtual bool CanFire() => true;

    public abstract void StopToFire();
}