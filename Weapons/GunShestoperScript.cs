using UnityEngine;
using System.Collections;

public class GunShestoperScript : BigGunAbstract
{
    private float heightGuns = 0.4f;
    private float lengthGuns = 1.3f;
    private float projectileSpeed = 15f;
    private Vector3 direction;
    private float lastShotUnit = 0;
    private float delayShotUnit = 0.15f;
    private int number = 0;

    public override void OpenFire(Vector3 Direction)
    {
        if ((lastFire + recharge) < Time.time)
        {
            commandToActivation = true;
            direction = Direction;
        }
    }

    private void Shoot(Vector3 Direction)
    {
        GameObject projectile = ObjectPool.Instance.TakeWithPool(ObjectPool.Instance.projectile);
        projectile.transform.localScale *= 0.66f;
        projectile.transform.position = this.gameObject.transform.position + new Vector3((Direction * lengthGuns).x, heightGuns, (Direction * lengthGuns).z);
        projectile.transform.rotation = Quaternion.identity;
        projectile.tag = "Projectile";
        projectile.transform.Rotate(Vector3.up, this.gameObject.transform.rotation.eulerAngles.y + 0);
        ProjectileScript ps = projectile.GetComponent<ProjectileScript>();
        ps.damage = damage;
        ps.soundEffectsOfHit = soundEffectsOfHit;
        ps.lifetime = 2.5f;
        ps.friendlyColliders = new System.Collections.Generic.List<Collider>();
        if (this.gameObject.tag == "Steamcopter")
        {
            ps.friendlyColliders = SteamcopterManager.Instance.collidersList;
        }
        if (this.gameObject.tag == "Enemy")
        {
            Transform trans = this.gameObject.transform.parent.parent;
            if (trans == null)
            {
                trans = this.gameObject.transform.parent;
            }
            ps.friendlyColliders.AddRange(trans.gameObject.GetComponentsInChildren<BoxCollider>());
        }
        Rigidbody rig = projectile.GetComponent<Rigidbody>();
        rig.velocity = new Vector3(0, 0, 0);
        rig.AddRelativeForce(Vector3.forward * projectileSpeed, ForceMode.Impulse);
        Rigidbody rigid = this.gameObject.transform.parent.GetComponent<Rigidbody>();
        if (rigid == null)
        {
            rigid = this.gameObject.transform.parent.parent.GetComponent<Rigidbody>();
        }
        rig.AddForce(rigid.velocity, ForceMode.Impulse);


        GameObject gameObjectEffectOfShot = (GameObject)Instantiate(effectOfShot, this.gameObject.transform.position + new Vector3((Direction * lengthGuns).x, heightGuns, (Direction * lengthGuns).z), Quaternion.identity);
        gameObjectEffectOfShot.transform.Rotate(Vector3.up, this.gameObject.transform.rotation.eulerAngles.y + 0);
        gameObjectEffectOfShot.transform.parent = this.gameObject.transform;
    }


    void Update()
    {
        if (!base.BasicUpdate())
            return;

        if (this.gameObject.tag == "Debris")
            return;

        if (commandToActivation)
        {
            if (TurnWeapon((direction - this.gameObject.transform.position)))
            {
                if ((lastShotUnit + delayShotUnit) < Time.time)
                {
                    if (number == 0)
                    {
                        PlaySoundEffectOfShot();
                    }
                    if (number < 6)
                    {
                        Shoot((direction - this.gameObject.transform.position).normalized);
                        number++;
                    }
                    else
                    {
                        commandToActivation = false;
                        lastFire = Time.time;
                        number = 0;
                    }
                    lastShotUnit = Time.time;
                }
            }
        }
    }
}
