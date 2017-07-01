using UnityEngine;
using System.Collections;

public abstract class BigGunAbstract : WeaponAbstract
{
    public AudioClip[] soundEffectsOfHit;
    public Vector2[] deathAreaList;

    public float damage;
    public float rotationSpeed;

    public override void Destroy()
    {
        SteamcopterManager.Instance.DestroyBigGun(this);
        base.Destroy();
    }

    public abstract void OpenFire(Vector3 Direction);

    protected virtual bool TurnWeapon(Vector3 Direction)
    {
        float angle = Vector3.Angle(Direction, Vector3.forward);
        if (Vector3.Angle(Direction, Vector3.left) < 90f)
        {
            angle = 360 - angle;
        }
        float globAngel = angle - this.gameObject.transform.parent.rotation.eulerAngles.y;
        if (globAngel < -180)
        {
            globAngel += 360;
        }

        if (globAngel > 180)
        {
            globAngel -= 360;
        }
        foreach (Vector2 vec in deathAreaList)
        {
            if ((vec.x <= globAngel) && (globAngel <= vec.y))
            {
                commandToActivation = false;
                return false;
            }
        }

        float angleRotation = CalculateAngleRotation(Direction);
        if (Mathf.Abs(angleRotation) < 1)
        {
            return true;
        }

        RotateWeaponModel(angleRotation, rotationSpeed);
        return false;
    }
}
