using UnityEngine;
using System.Collections;

public class RotationOfPropellerScript : MonoBehaviour
{
    public Vector3 axis;
    public float speed;
	
	void Update () 
    {
        this.gameObject.transform.Rotate(axis, speed * Time.deltaTime);	
	}
}
