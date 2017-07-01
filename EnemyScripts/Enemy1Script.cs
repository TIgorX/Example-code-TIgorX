using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy1Script : AbstractStateComponent 
{
    public float health = 500;
    public Rigidbody _rigidbody;
    public float shootingRange = 20;
    public float safeDistance = 20;
    public float speed = 0;
    private float maxSpeed = 6;
    public List<BigGunAbstract> bigGunList = new List<BigGunAbstract>();

    void Awake()
    {
        _rigidbody = this.gameObject.GetComponent<Rigidbody>();
        bigGunList.AddRange(this.gameObject.GetComponentsInChildren<BigGunAbstract>() );
    }

    void Update()
    {
        speed = _rigidbody.velocity.magnitude;
    }

    void FixedUpdate()
    {
        if (_rigidbody.velocity.magnitude > maxSpeed)
        {
            _rigidbody.velocity = this.gameObject.GetComponent<Rigidbody>().velocity.normalized * maxSpeed;
        }
    }

    public override void Destroy()
    {
        Destroy(this.gameObject);
    }

    public override void DealDamage(float Damage)
    {
        health -= Damage;
        if (health <= 0)
        {
            Destroy();
        }
    }

    public void ShootBigGun(Vector3 Direction)
    {
        foreach (BigGunAbstract bg in bigGunList)
        {
            bg.OpenFire(Direction);
        }
    }

    public void SetMoveDirectory(Vector3 moveDirectory)
    {
        float angle = Vector3.Angle(moveDirectory, Vector3.forward);
        if (Vector3.Angle(moveDirectory, Vector3.left) < 90f)
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

        _rigidbody.AddTorque(Mathf.Sign(angleRotation) * Vector3.up * Mathf.Abs(angleRotation) / 180 * Time.fixedDeltaTime * 6f, ForceMode.Impulse);
        float factor = 1 - Mathf.Abs(angleRotation) / 180;
        _rigidbody.AddRelativeForce(Vector3.forward * factor * Time.fixedDeltaTime * 4.5f, ForceMode.Impulse);
    }
}
