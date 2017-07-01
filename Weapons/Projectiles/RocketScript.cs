using UnityEngine;
using System.Collections;

public class RocketScript : MonoBehaviour
{
    public GameObject effectsOfIngesting;
    public AudioClip[] soundEffectsOfHit;
    public GameObject rocketMesh;

    public float damage = 20;
    private float creationTime;
    private float TimeOfInertness = 0.5f;
    private Rigidbody _rigidbody;
    public bool underTheSight = false;
    private bool flag = true;

    void Awake()
    {
        creationTime = Time.time;
        _rigidbody = this.gameObject.GetComponent<Rigidbody>();
    }

    public virtual void PlaySoundEffectOfHit()
    {
        if ((soundEffectsOfHit != null) && (soundEffectsOfHit.Length > 0))
        {
            AudioSource.PlayClipAtPoint(soundEffectsOfHit[Random.Range(0, soundEffectsOfHit.Length)], this.gameObject.transform.position);
        }
    }

    void OnEnable()
    {
        creationTime = Time.time;
        flag = true;
    }

    void FixedUpdate()
    {
        Vector3 vec3 = new Vector3( _rigidbody.velocity.x, _rigidbody.velocity.y, _rigidbody.velocity.z);
        Vector2 speedVector = new Vector2(0, vec3.y);
        vec3.y = 0;
        speedVector.x = vec3.magnitude;
        float angle = Vector2.Angle(Vector2.up, speedVector) - 90;
        rocketMesh.transform.localRotation = Quaternion.identity;
        rocketMesh.transform.Rotate(Vector3.right, angle);
    }

	void Update ()
    {
        if (this.gameObject.transform.position.y < -500)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((flag)  && ((creationTime + TimeOfInertness) < Time.time))
        {
            if ((other.gameObject.tag == "Enemy") ||
               (other.gameObject.tag == "EnemyBasis") ||
               (other.gameObject.tag == "Steamcopter")||
                (other.gameObject.tag == "Debris") ||
                (other.gameObject.tag == "Environment"))
            {
                if (other.gameObject.GetComponent<AbstractStateComponent>() != null)
                other.gameObject.GetComponent<AbstractStateComponent>().DealDamage(damage);
                flag = false;

                if (other.gameObject.GetComponent<SteamcopterArmorPlateScript>() == null)
                    PlaySoundEffectOfHit();
                GameObject gameObjectEffectsOfIngesting = (GameObject)Instantiate(effectsOfIngesting, this.gameObject.transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
        }
    }
}
