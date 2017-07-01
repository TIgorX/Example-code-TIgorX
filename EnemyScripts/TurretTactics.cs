using UnityEngine;
using System.Collections;

public class TurretTactics : EnemyAIControlComponent
{
    public float maxSafeDistance = 40;
    public float distanceActivity = 60;

    void Awake()
    {
        _rigidbody = this.gameObject.GetComponent<Rigidbody>();
    }

    public override void PerformActions()
    {
        if (AIBasic.isDead)
            return;

        bool flag = IsTargetInFieldOfVision();
        if (flag)
        {
            Vector3 direction = -this.gameObject.transform.position + ManagerAIEnemies.Instance.GetSteamcopterCoordinate();
            if ((direction.magnitude <= (maxSafeDistance + 5)))
            {
                AIBasic.ShootBigGun(ManagerAIEnemies.Instance.GetSteamcopterCoordinate() + (SteamcopterManager.Instance.steamcopterRigidbody.velocity * direction.magnitude / 15f));
                AIBasic.ShootRocket(ManagerAIEnemies.Instance.GetSteamcopterCoordinate() + (SteamcopterManager.Instance.steamcopterRigidbody.velocity * direction.magnitude / 8f));
            }
        }
    }
}
