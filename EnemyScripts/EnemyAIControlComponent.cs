using UnityEngine;
using System.Collections;

public abstract class EnemyAIControlComponent : MonoBehaviour 
{
    public EnemyAIBasic AIBasic;
    public Rigidbody _rigidbody;

    protected float factorTorque = 6;
    protected float factorForce = 4.5f;

    public void SetLineOfSight(Vector3 direction)
    {
        float angleRotation = CalculateAngleRotation(direction);
        _rigidbody.AddTorque(Mathf.Sign(angleRotation) * Vector3.up * Mathf.Abs(angleRotation) / 180 * Time.fixedDeltaTime * factorTorque, ForceMode.Impulse);
    }

    public void SetMoveDirectory(Vector3 moveDirection)
    {
        float angleRotation = CalculateAngleRotation(moveDirection);
        _rigidbody.AddTorque(Mathf.Sign(angleRotation) * Vector3.up * Mathf.Abs(angleRotation) / 180 * Time.fixedDeltaTime * factorTorque, ForceMode.Impulse);
        float factor = 1 - Mathf.Abs(angleRotation) / 180;
        _rigidbody.AddRelativeForce(Vector3.forward * factor * Time.fixedDeltaTime * factorForce, ForceMode.Impulse);
    }

    private float CalculateAngleRotation(Vector3 direction)
    {
        float angle = Vector3.Angle(direction, Vector3.forward);
        if (Vector3.Angle(direction, Vector3.left) < 90f)
        {
            angle = 360 - angle;
        }

        float angleRotation = +angle - this.gameObject.transform.rotation.eulerAngles.y;
        if (angleRotation < -180)
        {
            angleRotation += 360;
        }
        if (angleRotation > 180)
        {
            angleRotation -= 360;
        }
        return angleRotation;
    }

    protected bool IsTargetInFieldOfVision()
    {
        bool flag = true;
        RaycastHit[] RHList = Physics.RaycastAll(this.gameObject.transform.position, ManagerAIEnemies.Instance.GetSteamcopterCoordinate() 
            - this.gameObject.transform.position, (ManagerAIEnemies.Instance.GetSteamcopterCoordinate() - this.gameObject.transform.position).magnitude);
        foreach (RaycastHit rh in RHList)
        {
            if (rh.collider.gameObject.tag == "Environment")
            {
                flag = false;
                break;
            }
        }

        return flag;
    }

    public virtual void Destroy()
    {
    }

    public abstract void PerformActions();
}

public enum AIControlComponentType
{
    TortoiseTactics,
    BearTactics,
    Boss1Tactics,
    TurretTactics,
}
