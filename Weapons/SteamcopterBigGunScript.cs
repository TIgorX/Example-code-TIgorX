using UnityEngine;
using System.Collections;

public class SteamcopterBigGunScript : BigGunAbstract
{
    private float heightGuns = 0.4f;
    private float lengthGuns = 1.3f;
    private float projectileSpeed = 20f;
    private Vector3 direction;
    public float time111;

    void Awake()
    {
        recharge = 2f;
        damage = 10;
        rotationSpeed = 300f;
    }

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
        float st = Time.realtimeSinceStartup;
        GameObject projectile = ObjectPool.Instance.TakeWithPool(ObjectPool.Instance.projectile);
        time111 = Time.realtimeSinceStartup - st;
        projectile.transform.position = this.gameObject.transform.position + new Vector3((Direction * lengthGuns).x, heightGuns, (Direction * lengthGuns).z);
        projectile.transform.rotation = Quaternion.identity;
        projectile.tag = this.gameObject.tag + "Projectile";
        projectile.transform.Rotate(Vector3.up, this.gameObject.transform.rotation.eulerAngles.y + 0);
        projectile.GetComponent<ProjectileScript>().damage = damage;
        Rigidbody rig = projectile.GetComponent<Rigidbody>();
        rig.velocity = new Vector3(0, 0, 0);
        rig.AddRelativeForce(Vector3.forward * projectileSpeed, ForceMode.Impulse);
        rig.AddForce(this.gameObject.transform.parent.GetComponent<Rigidbody>().velocity, ForceMode.Impulse);

        GameObject gameObjectEffectOfShot = (GameObject)Instantiate(effectOfShot, this.gameObject.transform.position + new Vector3((Direction * lengthGuns).x, heightGuns, (Direction * lengthGuns).z), Quaternion.identity);
        gameObjectEffectOfShot.transform.Rotate(Vector3.up, this.gameObject.transform.rotation.eulerAngles.y + 0);
        gameObjectEffectOfShot.transform.parent = this.gameObject.transform;
    }

	void Update () 
    {
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
