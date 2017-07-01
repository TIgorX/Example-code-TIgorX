using UnityEngine;
using System.Collections;

public class TapToShootObjectScript : AbstractStateComponent
{
    public TrainingScript trainingScript;
    private Vector3 pointToMove;

    public void MoveToPoint(Vector3 Position)
    {
        pointToMove = Position + new Vector3(0, this.gameObject.transform.position.y, 0);
        this.gameObject.GetComponent<Rigidbody>().AddForce((pointToMove - this.gameObject.transform.position).normalized*4, ForceMode.VelocityChange);
    }


    public override void Destroy()
    {
        VisualizeEffectOfDestroying();
        trainingScript.DestroyedTapToShootObject();
        Destroy(this.gameObject);
    }

    public override void DealDamage(float Damage)
    {
        Destroy();
    }

	void FixedUpdate () 
    {
        if (((pointToMove - this.gameObject.transform.position).magnitude < 1) && (this.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 0.1f))
        {
            this.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
	
	}
}
