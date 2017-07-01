using UnityEngine;
using System.Collections;

public abstract class SuperEquipmentAbstract : WeaponAbstract
{
    public override void Destroy()
    {
        SteamcopterManager.Instance.DestroySuperEquipment(this);
        base.Destroy();
    }

    public abstract void Perform();
}
