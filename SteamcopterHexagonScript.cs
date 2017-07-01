using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SteamcopterHexagonScript : SectionOfShipScript
{
    public float additionalMass;
    public float additionalDrag;
    public float additionalAngularDrag;

    public CellNumber steamcopterHexagonNumber;
    public SteamcopterArmorPlateScript[] SteamcopterArmorPlateArray = new SteamcopterArmorPlateScript[6];

    public void RemoveArmorPlate(SteamcopterArmorPlateScript saps )
    {
        for (int i = 0; i < 6; i++)
        {
            if (SteamcopterArmorPlateArray[i] == saps)
            {
                SteamcopterArmorPlateArray[i] = null;
            }
        }
    }

    public void Sell()
    {
        SteamcopterManager.Instance.steamcopterCellArray[steamcopterHexagonNumber.x, steamcopterHexagonNumber.y] = null;
        if (superstructure != null)
        {
            superstructure.Destroy();
        }

        Rigidbody r = this.gameObject.transform.parent.GetComponent<Rigidbody>();
        r.mass -= additionalMass;
        r.drag -= additionalDrag;
        r.angularDrag -= additionalAngularDrag;

        additionalAngularDrag = 0;
        additionalDrag = 0;
        additionalMass = 0;

        Destroy(this.gameObject);
    }

    public override void Destroy()
    {
        Rigidbody r = this.gameObject.transform.parent.GetComponent<Rigidbody>();
        r.mass -= additionalMass;
        r.drag -= additionalDrag;
        r.angularDrag -= additionalAngularDrag;

        additionalAngularDrag = 0;
        additionalDrag = 0;
        additionalMass = 0;

        List<CellNumber> list = SteamcopterManager.Instance.GetFallsOffCells(steamcopterHexagonNumber);
        foreach (CellNumber cn in list)
        {
            if (SteamcopterManager.Instance.steamcopterCellArray[cn.x, cn.y] != null)
            {
                SteamcopterManager.Instance.steamcopterCellArray[cn.x, cn.y].Destroy();
            }
        }
        SteamcopterManager.Instance.steamcopterCellArray[steamcopterHexagonNumber.x, steamcopterHexagonNumber.y] = null;
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

    void OnDestroy()
    {
        SteamcopterManager.Instance.collidersList.Remove(this.gameObject.GetComponent<Collider>());
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
        Rigidbody r = this.gameObject.transform.parent.GetComponent<Rigidbody>();
        r.mass += additionalMass;
        r.drag += additionalDrag;
        r.angularDrag += additionalAngularDrag;
        currentHealth = health;
	}
}
