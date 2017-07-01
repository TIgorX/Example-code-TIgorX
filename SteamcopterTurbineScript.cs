using UnityEngine;
using System.Collections;

public class SteamcopterTurbineScript : AbstractStateComponent
{
    public float additionalLinearPowerOfEngine;
    public SteamcopterHexagonScript steamcopterHexagon;
    public float health = 25;

    public override void DealDamage(float Damage)
    {
        health -= Damage;
        if (health <= 0)
        {
            AudioSource.PlayClipAtPoint(BaseOfSounds.Instance.destroyingSuperstructure, this.gameObject.transform.position);
            VisualizeEffectOfDestroying().transform.parent = steamcopterHexagon.gameObject.transform;
            Destroy();
        }
    }
    public override void Destroy()
    {
        SteamcopterManager.Instance.linearPowerOfEngine -= additionalLinearPowerOfEngine;
        additionalLinearPowerOfEngine = 0;
        Destroy(this.gameObject);
    }

	void Start () 
    {
        SteamcopterManager.Instance.linearPowerOfEngine += additionalLinearPowerOfEngine;
	}
	
}
