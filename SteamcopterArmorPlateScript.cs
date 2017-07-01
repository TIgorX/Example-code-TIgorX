using UnityEngine;
using System.Collections;

public class SteamcopterArmorPlateScript : AbstractStateComponent
{
    public SteamcopterHexagonScript steamcopterHexagon;
    public AudioClip[] soundsOfHittingArmor;

    public float health = 200;

    public override void Destroy()
    {
        steamcopterHexagon.RemoveArmorPlate(this);
        Destroy(this.gameObject);
    }

    public virtual void PlaySoundEffectOfHit()
    {
        if ((soundsOfHittingArmor != null) && (soundsOfHittingArmor.Length > 0))
        {
            AudioSource.PlayClipAtPoint(soundsOfHittingArmor[Random.Range(0, soundsOfHittingArmor.Length)], this.gameObject.transform.position);
        }
    }

    void OnDestroy()
    {
        SteamcopterManager.Instance.collidersList.Remove(this.gameObject.GetComponent<Collider>());
    }

    public override void DealDamage(float Damage)
    {
        PlaySoundEffectOfHit();
        health -= Damage * 0.75f;
        steamcopterHexagon.DealDamage(0.25f * Damage);
        if (health <= 0)
        {
            AudioSource.PlayClipAtPoint(BaseOfSounds.Instance.destroyingSuperstructure, this.gameObject.transform.position);
            VisualizeEffectOfDestroying();
            Destroy();
        }
    }
}
