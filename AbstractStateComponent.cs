using UnityEngine;
using System.Collections;

public abstract class AbstractStateComponent : MonoBehaviour 
{
    public GameObject[] positionOfLowHPEffect;
    public GameObject lowHPEffect;
    public GameObject[] effectOfDestroying;

    private float lastSleepTime = 0;
    private float sleepDuration = 0;

    public abstract void DealDamage(float Damage);
    public abstract void Destroy();

    private bool isVisualizeLowHPEffect = false;

    public virtual GameObject VisualizeEffectOfDestroying()
    {
        GameObject gameObjectEffectOfDestroying = null;
        if ((effectOfDestroying != null) && (effectOfDestroying.Length > 0))
        {
            gameObjectEffectOfDestroying = (GameObject)Instantiate(effectOfDestroying[Random.Range(0, effectOfDestroying.Length)], this.gameObject.transform.position, Quaternion.identity);
        }
        return gameObjectEffectOfDestroying;
    }

    public virtual void VisualizeLowHPEffect()
    {
        if (isVisualizeLowHPEffect)
            return;
        isVisualizeLowHPEffect = true;

        if (this.gameObject.GetComponent<AudioSource>() != null)
        {
            GetComponent<AudioSource>().clip = BaseOfSounds.Instance.soundOfFire;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().Play();
        }

        if (positionOfLowHPEffect != null)
        foreach (GameObject go in positionOfLowHPEffect)
        {
            GameObject gameObjectEffectOfShot = (GameObject)Instantiate(lowHPEffect, go.transform.position, go.transform.rotation);
            gameObjectEffectOfShot.transform.parent = go.transform.parent;
        }
    }

    public virtual void Sleep(float duration)
    {
        lastSleepTime = Time.time;
        sleepDuration = duration;

        AbstractStateComponent[] ascList = this.gameObject.GetComponentsInChildren<AbstractStateComponent>();
        foreach (AbstractStateComponent asc in ascList)
        {
            if (asc != this)
            asc.Sleep(duration);
        }
    }

    public bool BasicUpdate()
    {
        if ((lastSleepTime  + sleepDuration) < Time.time)
        {
            return true;
        }
        return false;
    }
}
