using UnityEngine;
using System.Collections;

public class TortoiseTactics : EnemyAIControlComponent
{
    public float maxSafeDistance = 30;
    public float minSafeDistance = 10;
    public float distanceActivity = 80;

    private bool isFireMode = true;

    void Awake()
    {
        _rigidbody = this.gameObject.GetComponent<Rigidbody>();
    }

    public override void PerformActions()
    {
        if (AIBasic.isDead)
            return;

        if (ManagerAIEnemies.Instance.GetDistanceToTarget(AIBasic) > distanceActivity)
            return;

        Vector3 direction =-this.gameObject.transform.position + ManagerAIEnemies.Instance.GetDestinationPoint(AIBasic);
        bool flag = IsTargetInFieldOfVision();
        if (flag)
        {
            if (direction.magnitude <= minSafeDistance)
            {
                isFireMode = false;
            }
            if (direction.magnitude >= maxSafeDistance)
            {
                isFireMode = true;
            }
            if ((direction.magnitude <= (maxSafeDistance + 5)))
            {
                AIBasic.ShootBigGun(ManagerAIEnemies.Instance.GetSteamcopterCoordinate() + (SteamcopterManager.Instance.steamcopterRigidbody.velocity * direction.magnitude / 15f));
                AIBasic.ShootRocket(ManagerAIEnemies.Instance.GetSteamcopterCoordinate() + (SteamcopterManager.Instance.steamcopterRigidbody.velocity * direction.magnitude / 8f));
            }
            if ((direction.magnitude <= distanceActivity) &&
                (direction.magnitude > maxSafeDistance))
            {
                SetMoveDirectory(direction.normalized);
                return;
            }
            if ((isFireMode) && (direction.magnitude <= maxSafeDistance))
            {
                SetLineOfSight(direction.normalized);
                return;
            }
            if ((!isFireMode) && (direction.magnitude <= distanceActivity))
            {
                SetMoveDirectory((-direction).normalized);
                return;
            }
        }
    }

    void FixedUpdate()
    {

    }

	// Update is called once per frame
	void Update () {
	
	}

}
