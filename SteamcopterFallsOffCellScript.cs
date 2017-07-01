using UnityEngine;
using System.Collections;

public class SteamcopterFallsOffCellScript : MonoBehaviour 
{
    public Rigidbody _rigidbody;
    private float creationTime;

    void Awake()
    {
        _rigidbody = this.gameObject.GetComponent<Rigidbody>();
        creationTime = Time.time;
        Destroy(this.gameObject.GetComponent<BoxCollider>());
        GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 0.5f, ForceMode.Impulse);
    }

    void Update()
    {
        if (this.gameObject.transform.position.y < -500)
        {
            Destroy(this.gameObject);
        }
    }
}
