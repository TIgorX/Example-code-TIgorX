using UnityEngine;
using System.Collections;

public abstract class RocketAbstract : WeaponAbstract
{
    public AudioClip[] soundEffectsOfHit;

    public float damage;
    public float rotationSpeed;
    public int rocketsInHolder;
    protected int rocketsInHolderLeft;

    public override void Destroy()
    {
        SteamcopterManager.Instance.DestroyRocketLauncher(this);
        base.Destroy();
    }

    public abstract void OpenFire(Vector3 Direction);

    protected virtual bool TurnWeapon(Vector3 Direction)
    {
        float angleRotation = CalculateAngleRotation(Direction);
        if (Mathf.Abs(angleRotation) < 1)
        {
            return true;
        }

        RotateWeaponModel(angleRotation, rotationSpeed);
        return false;
    }
}
