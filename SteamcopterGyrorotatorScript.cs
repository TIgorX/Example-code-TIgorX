using UnityEngine;
using System.Collections;

public class SteamcopterGyrorotatorScript : AbstractStateComponent
{
    public float additionalrotationalPowerOfEngine;
    public SteamcopterHexagonScript steamcopterHexagon;
    public float health = 40;

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
        SteamcopterManager.Instance.rotationalPowerOfEngine -= additionalrotationalPowerOfEngine;
        additionalrotationalPowerOfEngine = 0;
        Destroy(this.gameObject);
    }

    void Start()
    {
        SteamcopterManager.Instance.rotationalPowerOfEngine += additionalrotationalPowerOfEngine;
    }
}
