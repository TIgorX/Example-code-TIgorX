using UnityEngine;
using System.Collections;

public abstract class AutoTurretAbstract : WeaponAbstract
{
    public AudioClip[] soundEffectsOfHit;

    public float range;
    public float damage;
    public float rotationSpeed;

    public override void Destroy()
    {
        SteamcopterManager.Instance.DestroyAutoTurret(this);
        base.Destroy();
    }

    public abstract void OpenFire(GameObject Direction);

    protected bool TurnWeapon(Vector3 Direction)
    {
        float angleRotation = CalculateAngleRotation(Direction);
        if (Mathf.Abs(angleRotation) < 3)
        {
            return true;
        }

        RotateWeaponModel(angleRotation, rotationSpeed);
        return false;
    }
}
