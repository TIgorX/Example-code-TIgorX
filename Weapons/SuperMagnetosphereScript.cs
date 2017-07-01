using UnityEngine;
using System.Collections;

public class SuperMagnetosphereScript : SuperEquipmentAbstract
{
    public GameObject wave;

    public float lifetime = 0.3f;
    public float expansionVelocityOfWave = 10;
    private float lastReflection = 0;

    private void ReflectProjectiles(float minRadius, float maxRadius)
    {
        Collider[] colliders = Physics.OverlapSphere(this.gameObject.transform.position, maxRadius);    
        for (int i = 0; i < colliders.Length; i++)
        {
            if ((colliders[i].gameObject.tag == "Projectile") ||
                (colliders[i].gameObject.tag == "EnemyRocket"))
            {
                if ((this.gameObject.transform.position - colliders[i].gameObject.transform.position).magnitude < minRadius)
                    continue;

                Rigidbody rigidbody = colliders[i].gameObject.GetComponent<Rigidbody>();
                Vector3 vec1 = this.gameObject.transform.position - colliders[i].gameObject.transform.position;
                vec1.y = 0;
                Vector3 vec2 = rigidbody.velocity;
                vec2.y = 0;

                if (vec2.magnitude < 0.01f)
                    continue;

                if (Vector3.Angle(vec1, vec2) >= 90)
                    continue;

                float lengthOffsetSpeedVector = Mathf.Sin(((90 - Vector3.Angle(vec1, vec2)) / 180f) * Mathf.PI) * vec2.magnitude * 2;
                Vector3 offsetSpeedVector;
                vec1 = -this.gameObject.transform.position + colliders[i].gameObject.transform.position;

                if (vec1.x != 0)
                {
                    float k = vec1.z / vec1.x;
                    float p = Mathf.Sqrt(Mathf.Pow(lengthOffsetSpeedVector, 2) / (k * k + 1)) * Mathf.Sign(vec1.x);
                    offsetSpeedVector = new Vector3(p, 0, p * k);
                }
                else
                {
                    offsetSpeedVector = new Vector3(0, 0, lengthOffsetSpeedVector);
                }
                rigidbody.AddForce(offsetSpeedVector, ForceMode.Impulse);

            }
        }
    }

    public override void Perform()
    {
        if ((lastFire + recharge) < Time.time)
        {
            commandToActivation = true;
            lastFire = Time.time;
            lastReflection = Time.time;
            wave.SetActive(true);
            PlaySoundEffectOfShot();     
        }
    }

    void FixedUpdate()
    {
        if (!base.BasicUpdate())
            return;

        if (commandToActivation)
        {
            if ((lastFire + lifetime) > Time.time)
            {
                float size = 2 * ((Time.time - lastFire) * expansionVelocityOfWave);
                wave.transform.localScale = new Vector3(size, size, size);
                wave.transform.rotation = Quaternion.identity;

                float minRadius = (lastReflection - lastFire) * expansionVelocityOfWave;
                float maxRadius = (Time.time - lastFire) * expansionVelocityOfWave;

                ReflectProjectiles(minRadius, maxRadius);
                lastReflection = Time.time;
            }
            else
            {
                commandToActivation = false;
                wave.SetActive(false);
                wave.transform.localScale = new Vector3(0, 0, 0);
            }
        }
    }
}
