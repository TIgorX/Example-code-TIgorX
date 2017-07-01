using UnityEngine;
using System.Collections;

public class PressWASDToMoveObjectScript : AbstractStateComponent
{
    private bool flag = false;
    private Vector3 pointToMove;
    public float health;

    public void MoveToPoint(Vector3 Position)
    {
        flag = true;
        pointToMove = Position + new Vector3(0, this.gameObject.transform.position.y, 0);
        this.gameObject.GetComponent<Rigidbody>().AddForce((pointToMove - this.gameObject.transform.position).normalized * 7f, ForceMode.VelocityChange);
    }

    public override void Destroy()
    {
        VisualizeEffectOfDestroying();
        Destroy(this.gameObject);
    }

    public override void DealDamage(float Damage)
    {
        health -= Damage;
        if (health <= 0)
        Destroy();

    }

    void FixedUpdate()
    {
        if (flag)
        {
            if (((pointToMove - this.gameObject.transform.position).magnitude < 1) && (this.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 0.1f))
            {
                this.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                flag = false;
            }
        }
    }
}
