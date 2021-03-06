﻿using UnityEngine;
using System.Collections;

public class UpgradeArmorPlateScript : AbstractUpgradeButtonScript 
{
    public UpgradeManager upgradeManager;
    public SteamcopterCellInfo cellInfo;
    public int position;
    public tk2dSlicedSprite slicedSprite;
    public tk2dUIItem uIItem;

    void Awake()
    {
        uIItem = this.gameObject.GetComponent<tk2dUIItem>();
        slicedSprite = this.gameObject.GetComponentInChildren<tk2dSlicedSprite>();
        uIItem.OnClickUIItem += OnClick;
    }


    void OnClick(tk2dUIItem Tk2dUIItem)
    {
        upgradeManager.UpgradeButtonClick(this);
    }

    public override void ActivateCheckSprite()
    {
        slicedSprite.SetSprite(1);
    }

    public override void DeactivateCheckSprite()
    {
        slicedSprite.SetSprite(0);
    }
}
