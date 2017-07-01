using UnityEngine;
using System.Collections;

public class MoveMaterialScript : MonoBehaviour 
{
    public Vector2 direction;
    public float rateDivider;

	void Update () 
    {
        this.gameObject.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", this.gameObject.GetComponent<Renderer>().material.GetTextureOffset("_MainTex") + (direction).normalized * Time.deltaTime / rateDivider);	
	}
}
