using UnityEngine;
using System.Collections;

public class AutoPointDefenseScript : AutoTurretAbstract
{
    private float heightGuns = 0.4f;
    private float lengthGuns = 1f;

    private Vector3 direction;
    private GameObject target;

    public override void OpenFire(GameObject Direction)
    {
        if ((lastFire + recharge) < Time.time)
        {
            commandToActivation = true;
            direction = (Direction.transform.position - this.gameObject.transform.position).normalized;
            target = Direction;
        }
    }

    private void Shoot(GameObject Target)
    {
        if (Target != null)
        {
            PlaySoundEffectOfShot();
            RocketScript rs = Target.GetComponent<RocketScript>();
            GameObject turretProjectile = (GameObject)Instantiate(BaseOfPrefabs.Instance.turretProjectile, this.gameObject.transform.position, Quaternion.identity);
            TurretScript ts = turretProjectile.GetComponent<TurretScript>();
            ts.effectsOfIngesting = rs.effectsOfIngesting;
            ts.target = Target;
            ts.soundEffectsOfHit = soundEffectsOfHit;
            Vector3 Direction = (Target.transform.position - this.gameObject.transform.position).normalized;
            GameObject gameObjectEffectOfShot = (GameObject)Instantiate(effectOfShot, this.gameObject.transform.position + new Vector3((Direction * lengthGuns).x, heightGuns, (Direction * lengthGuns).z), Quaternion.identity);
            gameObjectEffectOfShot.transform.Rotate(Vector3.up, this.gameObject.transform.rotation.eulerAngles.y);
            gameObjectEffectOfShot.transform.parent = this.gameObject.transform;

        }
    }

    void Update()
    {
        if (!base.BasicUpdate())
            return;

        if (commandToActivation)
        {
            if (TurnWeapon(direction))
            {
                Shoot(target);
                commandToActivation = false;
                lastFire = Time.time;
            }
        }
    }
}
