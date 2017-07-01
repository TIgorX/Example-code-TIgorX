using UnityEngine;
using System.Collections;

public class AutoJawsScript : AutoTurretAbstract
{
    private Animator animator;
    private Vector3 direction;
    private GameObject target;
    private Collider targetCollider;
    private float startAnimation = 0;
    private bool playAnimation = false;

    void Awake()
    {
        animator = this.gameObject.GetComponent<Animator>();
        animator.Play("JawsAnimation");
        animator.speed = 0;
      
    }

    public override void OpenFire(GameObject Direction)
    {
        if ((lastFire + recharge) < Time.time)
        {
            commandToActivation = true;
            direction = (Direction.transform.position - this.gameObject.transform.position).normalized;
            target = Direction;
            targetCollider = target.GetComponent<Collider>();
        }
    }

    private void Shoot(GameObject Target)
    {
        if (Target != null)
        {
            AbstractStateComponent asc = Target.GetComponent<AbstractStateComponent>();
            if (asc != null)
            {
                asc.DealDamage(damage);
                PlaySoundEffectOfShot();

                foreach (GameObject go in positionOfEffects)
                {
                    GameObject gameObjectEffectOfShot = (GameObject)Instantiate(effectOfShot, go.transform.position, go.transform.rotation);
                    gameObjectEffectOfShot.transform.parent = go.transform.parent;
                }
            }
        }

    }

    void Update()
    {
        if (!base.BasicUpdate())
            return;

        if (commandToActivation)
        {
            if ((target == null) ||(targetCollider == null))
            {
                commandToActivation = false;
                return;
            }
            if (TurnWeapon((target.gameObject.transform.position - this.gameObject.transform.position).normalized))
            {
                if (!playAnimation)
                {
                    playAnimation = true;
                    startAnimation = Time.time;
                    animator.speed = 3f / recharge;
                }
                if ((playAnimation) && (targetCollider.bounds.SqrDistance(this.gameObject.transform.position) <= (range * range)) && ((startAnimation + recharge / 2f) < Time.time))
                {
                    Shoot(target);
                    commandToActivation = false;
                    lastFire = Time.time;
                }
            }
        }
        if ((playAnimation) && ((startAnimation + recharge) < Time.time))
        {
            animator.speed = 0;
            playAnimation = false;
            commandToActivation = false;
        }
    }
}

