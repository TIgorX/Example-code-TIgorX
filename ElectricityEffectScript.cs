using UnityEngine;
using System.Collections;

public class ElectricityEffectScript : MonoBehaviour 
{
    private float creationTime;
    public float lifetime = 2;

    private GameObject origin = null;
    private GameObject target = null;
    private Vector3 originOffset;
    private Vector3 targetOffset;
    private Vector3 normalLocalScale;

    public void Init(GameObject Origin, Vector3 OriginOffset, GameObject Target, Vector3 TargetOffset)
    {
        origin = Origin;
        target = Target;
        originOffset = OriginOffset;
        targetOffset = TargetOffset;
    }

    void Awake()
    {
        creationTime = Time.time;
        normalLocalScale = this.gameObject.transform.localScale;
    }

	void Update ()
    {

        if ((creationTime + lifetime) < Time.time)
        {
            Destroy(this.gameObject);
            return;
        }

        if ((origin != null) && (target != null))
        {
            this.gameObject.transform.position = (origin.transform.position + originOffset + target.transform.position + targetOffset) / 2f;
            this.gameObject.transform.localScale = normalLocalScale * (-(origin.transform.position + originOffset) + (target.transform.position + targetOffset)).magnitude;
            this.gameObject.transform.rotation = Quaternion.identity;

            float angle = Vector3.Angle((-(origin.transform.position + originOffset) + (target.transform.position + targetOffset)), Vector3.forward);
            if (Vector3.Angle((-(origin.transform.position + originOffset) + (target.transform.position + targetOffset)), Vector3.left) < 90f)
            {
                angle = 360 - angle;
            }
            this.gameObject.transform.Rotate(Vector3.up, angle);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }
}

