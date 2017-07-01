using UnityEngine;
using System.Collections;

public class SectionOfShipScript : AbstractStateComponent
{
    public AbstractStateComponent superstructure;
    public float health = 100;
    protected float currentHealth;

    public override void Destroy()
    {
        if (superstructure != null)
        {
            superstructure.Destroy();
        }

        this.gameObject.transform.parent = null;
        this.gameObject.GetComponent<BoxCollider>().isTrigger = true;
        this.gameObject.AddComponent<Rigidbody>();
        this.gameObject.AddComponent<SteamcopterFallsOffCellScript>();
        Destroy(this);
    }

    public override void DealDamage(float Damage)
    {
        if (superstructure != null)
        {
            superstructure.DealDamage(Damage);
            return;
        }
        currentHealth -= Damage;

        if (currentHealth <= health / 2)
            VisualizeLowHPEffect();

        if (currentHealth <= 0)
        {
            AudioSource.PlayClipAtPoint(BaseOfSounds.Instance.destroyingSuperstructure, this.gameObject.transform.position);
            Destroy();
        }
    }

	void Start () 
    {
        currentHealth = health;	
	}
}
