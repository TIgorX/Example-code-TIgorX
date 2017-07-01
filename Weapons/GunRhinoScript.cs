using UnityEngine;
using System.Collections;

public class GunRhinoScript : BigGunAbstract
{
    private float heightGuns = 0.4f;
    private float lengthGuns = 1f;
    private float projectileSpeed = 15f;
    private Vector3 direction;

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
        PlaySoundEffectOfShot();       
        GameObject projectile = ObjectPool.Instance.TakeWithPool(ObjectPool.Instance.projectile);
        projectile.transform.position = this.gameObject.transform.position + new Vector3((Direction * lengthGuns).x, heightGuns, (Direction * lengthGuns).z);
        projectile.transform.rotation = Quaternion.identity;
        projectile.tag = "Projectile";
        projectile.transform.Rotate(Vector3.up, this.gameObject.transform.rotation.eulerAngles.y + 0);
        ProjectileScript ps = projectile.GetComponent<ProjectileScript>();
        ps.damage = damage;
        ps.soundEffectsOfHit = soundEffectsOfHit;
        ps.friendlyColliders = new System.Collections.Generic.List<Collider>();
        if (this.gameObject.tag == "Steamcopter")
        {
            ps.friendlyColliders.AddRange(SteamcopterManager.Instance.gameObject.GetComponentsInChildren<BoxCollider>());
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

        if (commandToActivation)
        {
            if (TurnWeapon((direction - this.gameObject.transform.position)))
            {
                Shoot((direction - this.gameObject.transform.position).normalized);
                commandToActivation = false;
                lastFire = Time.time;
            }
        }
    }
}

