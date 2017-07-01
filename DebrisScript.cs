using UnityEngine;
using System.Collections;

public class DebrisScript : MonoBehaviour 
{
    private bool flag = true;
    private bool activityTimeExpired = false;
    public Rigidbody _rigidbody;
    private float creationTime;
    private float activityTime = 2;
    public int income = -1;

    void Awake()
    {
        creationTime = Time.time;
    }

	void Update () 
    {
        if ((!activityTimeExpired) && ((creationTime + activityTime) < Time.time))
        {
            _rigidbody.useGravity = true;
            activityTimeExpired = true;
        }

        if (this.gameObject.transform.position.y < -500)
        {
            Destroy(this.gameObject);
        }
	}
}
