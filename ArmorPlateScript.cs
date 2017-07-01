using UnityEngine;
using System.Collections;

public class ArmorPlateScript : SteamcopterArmorPlateScript 
{
    public AbstractStateComponent abstractStateComponent;
	void Start () {
	
	}
	
	void Update () {
	
	}

    public override void DealDamage(float Damage)
    {
        PlaySoundEffectOfHit();
        health -= Damage * 0.75f;
        abstractStateComponent.DealDamage(0.25f * Damage);
        if (health <= 0)
        {
            AudioSource.PlayClipAtPoint(BaseOfSounds.Instance.destroyingSuperstructure, this.gameObject.transform.position);
            VisualizeEffectOfDestroying();
            Destroy(this.gameObject);
        }
    }
}
