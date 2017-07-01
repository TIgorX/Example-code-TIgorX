using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileScript : MonoBehaviour 
{
    public GameObject effectsOfIngesting;
    public AudioClip[] soundEffectsOfHit;

    public float damage = 20;
    private float creationTime;
    public float lifetime = 4;
    public List<Collider> friendlyColliders;
    private bool inFriendlyColliders = true;
    private bool flag = true;
    private Collider _collider;

    public virtual void PlaySoundEffectOfHit()
    {
        if ((soundEffectsOfHit != null) && (soundEffectsOfHit.Length > 0))
        {
            AudioSource.PlayClipAtPoint(soundEffectsOfHit[Random.Range(0, soundEffectsOfHit.Length)], this.gameObject.transform.position);
        }
    }

    void Awake()
    {
        creationTime = Time.time;
        _collider = this.gameObject.GetComponent<BoxCollider>();
    }

    void OnEnable()
    {
        creationTime = Time.time;
        flag = true;
        inFriendlyColliders = true;
        friendlyColliders = null;
        soundEffectsOfHit = null;
        this.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

	void Update ()
    {
        if ((creationTime + lifetime) < Time.time)
        {
            ObjectPool.Instance.ReturnToPool(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        if (inFriendlyColliders)
        {
            bool Bool = true;
            foreach (Collider col in friendlyColliders)
            {
                if ((col != null) && (col.bounds.Intersects(_collider.bounds)))
                {
                    Bool = false;
                    break;
                }
            }
            if (Bool)
            {
                inFriendlyColliders = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((flag))
        {
            if ((other.gameObject.tag == "Enemy") ||
               (other.gameObject.tag == "EnemyBasis") ||
               (other.gameObject.tag == "Steamcopter") ||
                (other.gameObject.tag == "Debris") ||
                (other.gameObject.tag == "Environment"))
            {
                if (inFriendlyColliders)
                {
                    foreach (Collider col in friendlyColliders)
                    {
                        if ((col != null) && (col == other))
                        {
                            return;
                        }
                    }
                }

                if (other.gameObject.GetComponent<AbstractStateComponent>() != null)
                other.gameObject.GetComponent<AbstractStateComponent>().DealDamage(damage);

                flag = false;
                GameObject gameObjectEffectsOfIngesting = (GameObject)Instantiate(effectsOfIngesting, this.gameObject.transform.position, Quaternion.identity);
                if (other.gameObject.GetComponent<SteamcopterArmorPlateScript>()==null)
                PlaySoundEffectOfHit();
                ObjectPool.Instance.ReturnToPool(this.gameObject);
            }
        }
    }
}
