using UnityEngine;
using System.Collections;

public class RocketProtuberanetsScript : RocketAbstract
{
    private float heightGuns = 1f;
    private float lengthGuns = 0.7f;
    private float angleOfShot = (40f / 180f) * Mathf.PI;
    private Vector3 direction;
    private float lastShotUnit = 0;
    private float delayShotUnit = 0.5f;
    private int currentBulletPosition = 0;

    void Awake()
    {
        rocketsInHolderLeft = rocketsInHolder;
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
        Direction.y = 0;
        PlaySoundEffectOfShot();
        if ((Direction.magnitude - this.gameObject.transform.position.y) <= 0)
        {
            commandToActivation = false;
            return;
        }

        float projectileSpeed = Mathf.Sqrt(((Direction.magnitude - this.gameObject.transform.position.y) * -Physics.gravity.y) / (Mathf.Sin(2*angleOfShot)));

        GameObject projectile = (GameObject)Instantiate(BaseOfPrefabs.Instance.rocket, this.gameObject.transform.position, Quaternion.identity);
        if (positionOfBullets.Length > 0)
        {
            projectile.transform.position = positionOfBullets[currentBulletPosition].transform.position;
            currentBulletPosition++;
            currentBulletPosition = currentBulletPosition % positionOfBullets.Length; 
        }
        else
        {
            projectile.transform.position = this.gameObject.transform.position + new Vector3((Direction.normalized * lengthGuns).x, heightGuns, (Direction.normalized * lengthGuns).z);
        }
        projectile.transform.rotation = Quaternion.identity;
        projectile.tag = this.gameObject.tag + "Rocket";
        projectile.transform.Rotate(Vector3.up, this.gameObject.transform.rotation.eulerAngles.y + 0);
        RocketScript rs = projectile.GetComponent<RocketScript>();
        rs.damage = damage;
        rs.soundEffectsOfHit = soundEffectsOfHit;
        Rigidbody rig = projectile.GetComponent<Rigidbody>();
        rig.velocity = new Vector3(0, 0, 0);
        rig.AddRelativeForce((new Vector3(0, 1 * Mathf.Sin(angleOfShot), 1 * Mathf.Cos(angleOfShot))) * projectileSpeed, ForceMode.Impulse);
        Rigidbody rigid = this.gameObject.transform.parent.GetComponent<Rigidbody>();
        if (rigid == null)
        {
            rigid = this.gameObject.transform.parent.parent.GetComponent<Rigidbody>();
        }
        rig.AddForce(rigid.velocity, ForceMode.Impulse);

        GameObject gameObjectEffectOfShot = (GameObject)Instantiate(effectOfShot, this.gameObject.transform.position, Quaternion.identity);
        if (positionOfBullets.Length > 0)
        {
            gameObjectEffectOfShot.transform.position = positionOfBullets[currentBulletPosition].transform.position;
        }
        else
        {
            gameObjectEffectOfShot.transform.position = this.gameObject.transform.position + new Vector3((Direction.normalized * lengthGuns).x, heightGuns, (Direction.normalized * lengthGuns).z);
        }
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
                    Shoot((direction - this.gameObject.transform.position));
                    commandToActivation = false;
                    rocketsInHolderLeft--;

                    if (rocketsInHolderLeft == 0)
                    {
                        lastFire = Time.time;
                        rocketsInHolderLeft = rocketsInHolder;
                    }
                    lastShotUnit = Time.time;
                }
            }
        }
    }
}

