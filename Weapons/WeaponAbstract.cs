using UnityEngine;
using System.Collections;

public abstract class WeaponAbstract : AbstractStateComponent
{
    public GameObject effectOfShot;
    public GameObject[] positionOfEffects;
    public AudioClip[] soundEffectsOfShot;
    public GameObject[] positionOfBullets;
    public SectionOfShipScript sectionOfShip;

    public float health = 40;
    public float recharge;
    protected bool commandToActivation = false;
    protected float lastFire = 0;

    public virtual void PlaySoundEffectOfShot()
    {
        if ((soundEffectsOfShot != null) && (soundEffectsOfShot.Length > 0))
        {
            AudioSource.PlayClipAtPoint(soundEffectsOfShot[Random.Range(0, soundEffectsOfShot.Length)], this.gameObject.transform.position);
        }
    }

    public override void Destroy()
    {
        if (sectionOfShip != null)
        sectionOfShip.superstructure = null;
        Destroy(this.gameObject);
    }

    public override void DealDamage(float Damage)
    {
        health -= Damage;
        if (health <= 0)
        {
            AudioSource.PlayClipAtPoint(BaseOfSounds.Instance.destroyingSuperstructure, this.gameObject.transform.position);
            if (sectionOfShip != null)
                VisualizeEffectOfDestroying().transform.parent = sectionOfShip.gameObject.transform;
            Destroy();
        }
    }

    public virtual float GetTimeLeftToRecharge()
    {
        float timeLeft = -1;
        if ((lastFire + recharge) < Time.time)
        {
            timeLeft = 0;
            if (commandToActivation)
            {
                timeLeft = recharge;
            }
        }
        else
        {
            timeLeft = Time.time - lastFire;
        }
        return timeLeft;
    }

    public override void Sleep(float duration)
    {
        base.Sleep(duration);
        lastFire += duration;
    }

    protected virtual float CalculateAngleRotation(Vector3 Direction)
    {
        float angle = Vector3.Angle(Direction, Vector3.forward);
        if (Vector3.Angle(Direction, Vector3.left) < 90f)
        {
            angle = 360 - angle;
        }
        float angleRotation = +angle - this.gameObject.transform.rotation.eulerAngles.y;
        if (angleRotation < -180)
        {
            angleRotation += 360;
        }
        if (angleRotation > 180)
        {
            angleRotation -= 360;
        }
        return angleRotation;
    }

    protected virtual void RotateWeaponModel(float AngleRotation, float RotationSpeed)
    {
        float rotationPerUpdate = RotationSpeed * Time.deltaTime;
        if (rotationPerUpdate > Mathf.Abs(AngleRotation))
            rotationPerUpdate = Mathf.Abs(AngleRotation);
        this.gameObject.transform.Rotate(Vector3.up * Mathf.Sign(AngleRotation), rotationPerUpdate);
    }

}
