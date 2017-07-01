using UnityEngine;
using System.Collections;

public class AutoSuppressorScript : AutoTurretAbstract
{
    public float sleepTime;

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
            AbstractStateComponent asc = Target.GetComponent<AbstractStateComponent>();
            if (asc != null)
            {
                asc.Sleep(sleepTime);
                PlaySoundEffectOfShot();
                GameObject gameObjectEffectOfShot = (GameObject)Instantiate(effectOfShot, (this.gameObject.transform.position + Target.transform.position) / 2f, Quaternion.identity);
                ElectricityEffectScript ees = gameObjectEffectOfShot.GetComponent<ElectricityEffectScript>();
                ees.Init(this.gameObject, new Vector3(0,1,0), Target, Vector3.zero);
            }
        }

    }

    void Update()
    {
        if (!base.BasicUpdate())
            return;

        if (commandToActivation)
        {
            Shoot(target);
            commandToActivation = false;
            lastFire = Time.time;
        }
    }
}

