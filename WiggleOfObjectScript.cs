using UnityEngine;
using System.Collections;

public class WiggleOfObjectScript : MonoBehaviour 
{
    public Vector3 rangePoint1;
    public Vector3 rangePoint2;
    public float speed;
    private Vector3 rangePosition1;
    private Vector3 rangePosition2;

    private bool activeFirstRangePoint = true;

    void Awake()
    {
        rangePosition1 = this.gameObject.transform.position + rangePoint1;
        rangePosition2 = this.gameObject.transform.position + rangePoint2;
        this.gameObject.transform.position = rangePosition1 + (rangePosition2 - rangePosition1) * Random.Range(0f, 1f);
    }

	
    void Update()
    {
        Vector3 direction;
        if (activeFirstRangePoint)
        {
            direction = rangePosition1 - this.gameObject.transform.position;
        }
        else
        {
            direction = rangePosition2 - this.gameObject.transform.position;
        }
        if ((activeFirstRangePoint) && ((rangePosition1 - this.gameObject.transform.position).magnitude < 0.1f))
        {
            activeFirstRangePoint = false;
        }
        if ((!activeFirstRangePoint) && ((rangePosition2 - this.gameObject.transform.position).magnitude < 0.1f))
        {
            activeFirstRangePoint = true;
        }
        this.gameObject.transform.position = this.gameObject.transform.position + direction.normalized * Time.deltaTime * speed;
    }
}
