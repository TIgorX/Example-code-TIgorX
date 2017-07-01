using UnityEngine;
using System.Collections;

public class Boss1Tactics : EnemyAIControlComponent
{
    public float maxSafeDistance = 40;
    public float distanceActivity = 150;

    void Awake()
    {
        _rigidbody = this.gameObject.GetComponent<Rigidbody>();
        factorForce *= 80;
        factorTorque *= 150;
    }

    public override void Destroy()
    {
        ManagerAIEnemies.Instance.BossDestroyed();
    }

    public override void PerformActions()
    {
        if (AIBasic.isDead)
            return;

        if (ManagerAIEnemies.Instance.GetDistanceToTarget(AIBasic) > distanceActivity)
            return;

        Vector3 direction = -this.gameObject.transform.position + ManagerAIEnemies.Instance.GetDestinationPoint(AIBasic);
        bool flag = IsTargetInFieldOfVision();
        if (flag)
        {
            direction = -this.gameObject.transform.position + ManagerAIEnemies.Instance.GetSteamcopterCoordinate();
            if ((direction.magnitude <= (maxSafeDistance + 20)))
            {
                AIBasic.ShootBigGun(ManagerAIEnemies.Instance.GetSteamcopterCoordinate() + (SteamcopterManager.Instance.steamcopterRigidbody.velocity * direction.magnitude / 15f));

                if (direction.magnitude > 15f)
                    AIBasic.ShootRocket(ManagerAIEnemies.Instance.GetSteamcopterCoordinate() + (SteamcopterManager.Instance.steamcopterRigidbody.velocity * direction.magnitude / 8f));
            }
            if ((direction.magnitude <= distanceActivity) &&
                (direction.magnitude > maxSafeDistance))
            {
                SetMoveDirectory(direction.normalized);
                return;
            }
            if ((direction.magnitude <= maxSafeDistance))
            {               
                SetLineOfSight(direction.normalized);
                return;
            }
        }
        else
        {
            SetMoveDirectory(direction.normalized);
        }
    }
}
