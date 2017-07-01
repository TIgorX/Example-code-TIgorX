using UnityEngine;
using System.Collections;

public class TurretScript : MonoBehaviour 
{
    public GameObject effectsOfIngesting;
    public GameObject target;
    public AudioClip[] soundEffectsOfHit;

    public float damage;
    public float speed;
    private float allowableDistance = 0.5f;
    private bool flag = true;

    public virtual void PlaySoundEffectOfHit()
    {
        if ((soundEffectsOfHit != null) && (soundEffectsOfHit.Length > 0))
        {
            AudioSource.PlayClipAtPoint(soundEffectsOfHit[Random.Range(0, soundEffectsOfHit.Length)], this.gameObject.transform.position);
        }
    }

    void Update()
    {
        if (flag)
        {
            if ((target == null))
            {
                Destroy(this.gameObject);
                flag = false;
                return;
            }

            if ((target.transform.position - this.gameObject.transform.position).magnitude < allowableDistance)
            {
                AbstractStateComponent asc = target.GetComponent<AbstractStateComponent>();
                RocketScript rs = target.GetComponent<RocketScript>();
                if (rs != null)
                {
                    Destroy(target);
                    GameObject gameObjectEffectsOfIngesting = (GameObject)Instantiate(effectsOfIngesting, this.gameObject.transform.position, Quaternion.identity);
                    PlaySoundEffectOfHit();
                }
                if (asc != null)
                {
                    asc.DealDamage(damage);
                }
                flag = false;
                Destroy(this.gameObject);
                return;
            }

            {
                if ((target.transform.position - this.gameObject.transform.position).magnitude < ((target.transform.position - this.gameObject.transform.position).normalized * Time.deltaTime * speed).magnitude)
                {
                    this.gameObject.transform.position = target.transform.position;
                }
                else
                {
                    this.gameObject.transform.position += (target.transform.position - this.gameObject.transform.position).normalized * Time.deltaTime * speed;
                }
                return;
            }
        }
    }
}

