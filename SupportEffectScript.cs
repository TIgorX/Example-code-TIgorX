using UnityEngine;
using System.Collections;

public class SupportEffectScript : MonoBehaviour 
{
    private float creationTime;
    public float lifetime = 5;
    private bool flag = true;

	void Start () 
    {
        creationTime = Time.time;
	}
	
	void Update () 
    {
        if ((flag)  && ((creationTime + lifetime) < Time.time))
        {
            flag = false;
            Destroy(this.gameObject);
        }
	}
}
