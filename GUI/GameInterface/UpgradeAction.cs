using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AbstractUpgradeAction
{
    public bool isActive = false;
    public static UpgradeManager upgradeManager;
    public UpgradeActionID actionID;
    public int cost = 0;
    public InfoPanelData infoPanelData;
    public bool activatedNewUpgradeActionList;
    protected List<AbstractUpgradeAction> activatedUpgradeActionList;

    public virtual void CreateActivatedUpgradeActionList()
    {
        activatedUpgradeActionList = new List<AbstractUpgradeAction>();
    }

    public virtual bool ConfirmationMode(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        upgradeManager.RemoveLastHistoryEvent();
        upgradeButtonScript.ActivateCheckSprite();

        bool flag = isActive;
        isActive = true;

        foreach (AbstractUpgradeAction aua in upgradeManager.activeUpgradeActionList)
        {
            if (aua.actionID != actionID)
            {
                aua.isActive = false;
                if (upgradeManager.lastActiveUpgradeButtonScript != null)
                {
                    upgradeManager.lastActiveUpgradeButtonScript.DeactivateCheckSprite();
                }
            }
        }
        upgradeManager.lastActiveUpgradeButtonScript = (UpgradeButtonScript)upgradeButtonScript;
        if (!flag)
        {
            TrainingScript.Instance.SelectedModule();
        }
        else
        {
            TrainingScript.Instance.AddedNewModule();
        }
        return flag;
    }

    public virtual bool CanBeActive(SteamcopterCellInfo cellInfo)
    {
        return true;
    }
    public virtual List<AbstractUpgradeAction> GetActivatedUpgradeActionList()
    {
        return activatedUpgradeActionList;
    }
    public virtual void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        CreateActivatedUpgradeActionList();
    }
}

public struct InfoPanelData
{
    public string infoPanelText;

    public InfoPanelData(string InfoPanelText)
    {
        infoPanelText = InfoPanelText;
    }
}

public enum UpgradeActionID
{
    emptyCellClick,
    cellClick,
    engineCategoryClick,
    weaponCategoryClick,
    protectionCategoryClick,
    upgradeCategoryClick,
    backClick,
    weaponCategory_smallGunClick,
    weaponCategory_bigGunClick,
    sellClick,
    protectionCategory_SelectArmorPlateClick,
    engineCategory_GyrorotatorClick,
    engineCategory_TurbineClick,
    protectionCategory_PointDefenseClick,
    protectionCategory_ArmorPlateClick,
    weaponCategory_RocketProtuberanetsClick,
    weaponCategory_JawsClick,
    superEquipmentCategoryClick,
    superEquipmentCategory_MagnetosphereClick,
    protectionCategory_SuppressorClick,
    weaponCategory_RhinoClick,
}

public class EmptyCellClick : AbstractUpgradeAction
{
    public CellNumber activeCell;
    public EmptyCellClick()
    {
        infoPanelData = new InfoPanelData("Select the cell to add new module or sell existing one");
        actionID = UpgradeActionID.emptyCellClick;
        activatedNewUpgradeActionList = false;
        activeCell = new CellNumber(-1, -1);
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);
        upgradeManager.upgradeUserInterface.scrollableAreaGameObject.SetActive(false);
        UpgradeHexagonalCellScript uhcs = (UpgradeHexagonalCellScript)upgradeButtonScript;
        if ((activeCell.Equals(new CellNumber(-1, -1))) || (!activeCell.Equals(uhcs.cellInfo.cellNumber)))
        {
            uhcs.ActivateCheckSprite();
            upgradeManager.SetActiveUpgradeHexagonalCellScript(uhcs);
            activeCell = uhcs.cellInfo.cellNumber;
            infoPanelData = new InfoPanelData("Add cell here?\n cost = " + PlayerInfoManager.GetHexagonalCellCost(uhcs.cellInfo.cellNumber));

            TrainingScript.Instance.SelectedNewCell();
        
        }
        else
        {

            if (activeCell.Equals(uhcs.cellInfo.cellNumber))
            {

                TrainingScript.Instance.AddedNewCell();

                if (PlayerInfoManager.resources >= PlayerInfoManager.GetHexagonalCellCost(uhcs.cellInfo.cellNumber))
                {
                    //Debug.Log("Create new cell");
                    activatedNewUpgradeActionList = true;
                    PlayerInfoManager.AddHexagonalCell(uhcs.cellInfo.cellNumber);
                    upgradeManager.upgradeUserInterface.AddHexagonalCell(new SteamcopterCellInfo(uhcs.cellInfo.cellNumber, HexagonSuperstructureType.none, HexagonSuperstructure.none, new List<int>()));
                }
                else
                {
                    infoPanelData = new InfoPanelData("not enough resources");            
                }
            }
        }
    }
}

public class CellClick : AbstractUpgradeAction
{
    public CellClick()
    {
        infoPanelData = new InfoPanelData("Select the equipment to install");
        actionID = UpgradeActionID.cellClick;
        activatedNewUpgradeActionList = true;
    }

    public override void CreateActivatedUpgradeActionList()
    {
        activatedUpgradeActionList = new List<AbstractUpgradeAction>()
        {
            new EngineCategoryClick(), 
            new WeaponCategoryClick(), 
            new ProtectionCategoryClick(),
            //new UpgradeCategoryClick(),
            //new SuperEquipmentCategoryClick(),
            new CellClick(),
            new EmptyCellClick(),
            new SellClick(),
            new BackClick()
        };
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);

        upgradeManager.historyUpgradeAction.RemoveRange(0, upgradeManager.historyUpgradeAction.Count - 1);
        upgradeManager.historyUpgradeButtonScript.RemoveRange(0, upgradeManager.historyUpgradeButtonScript.Count - 1);

        UpgradeHexagonalCellScript uhcs = (UpgradeHexagonalCellScript)upgradeButtonScript;
        upgradeManager.SetActiveUpgradeHexagonalCellScript(uhcs);
        uhcs.ActivateActiveSprite();


    }
}

public class EngineCategoryClick : AbstractUpgradeAction
{
    public EngineCategoryClick()
    {
        infoPanelData = new InfoPanelData("Select the equipment to install");
        actionID = UpgradeActionID.engineCategoryClick;
        activatedNewUpgradeActionList = true;
    }

    public override void CreateActivatedUpgradeActionList()
    {
        activatedUpgradeActionList = new List<AbstractUpgradeAction>()
        {
            new CellClick(),
            new EngineCategory_TurbineClick(),
            new EngineCategory_GyrorotatorClick(),
            new BackClick(),
            new EmptyCellClick()
        };
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);
        TrainingScript.Instance.SelectedCategory();
    }

    public override bool CanBeActive(SteamcopterCellInfo cellInfo)
    {
        if (cellInfo.superstructureType == HexagonSuperstructureType.none)
        {
            TrainingScript.Instance.SelectedFreeCell();
            return true;
        }
        return false;
    }
}

public class WeaponCategoryClick : AbstractUpgradeAction
{
    public WeaponCategoryClick()
    {
        infoPanelData = new InfoPanelData("Select the equipment to install");
        actionID = UpgradeActionID.weaponCategoryClick;
        activatedNewUpgradeActionList = true;
    }

    public override void CreateActivatedUpgradeActionList()
    {
        activatedUpgradeActionList = new List<AbstractUpgradeAction>()
        {
            new WeaponCategory_smallGunClick(),
            new WeaponCategory_RhinoClick(),
            new WeaponCategory_bigGunClick(),
            new WeaponCategory_RocketProtuberanetsClick(),
            new WeaponCategory_JawsClick(),
            new BackClick(),
            new CellClick(),
            new EmptyCellClick()
        };
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);
        TrainingScript.Instance.SelectedCategory();
    }

    public override bool CanBeActive(SteamcopterCellInfo cellInfo)
    {
        if (cellInfo.superstructureType == HexagonSuperstructureType.none)
        {
            return true;
        }
        return false;
    }
}

public class ProtectionCategoryClick : AbstractUpgradeAction
{
    public ProtectionCategoryClick()
    {
        infoPanelData = new InfoPanelData("Select the equipment to install");
        actionID = UpgradeActionID.protectionCategoryClick;
        activatedNewUpgradeActionList = true;
    }

    public override void CreateActivatedUpgradeActionList()
    {
        activatedUpgradeActionList = new List<AbstractUpgradeAction>()
        {
            new ProtectionCategory_ArmorPlateClick(),
            new ProtectionCategory_PointDefenseClick(),
            new ProtectionCategory_SuppressorClick(),
            new SuperEquipmentCategory_MagnetosphereClick(),
            new BackClick(),
            new CellClick(),
            new EmptyCellClick()
        };
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);


        //upgradeManager.upgradeUserInterface.VisualizeUpgradeArmorPlate(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo);
    }

    public override bool CanBeActive(SteamcopterCellInfo cellInfo)
    {
        if (cellInfo.superstructureType == HexagonSuperstructureType.none)
        {
            return true;
        }
        return false;
    }
}

public class UpgradeCategoryClick : AbstractUpgradeAction
{
    public UpgradeCategoryClick()
    {
        infoPanelData = new InfoPanelData("Select upgrade");
        actionID = UpgradeActionID.upgradeCategoryClick;
        activatedNewUpgradeActionList = true;
    }

    public override void CreateActivatedUpgradeActionList()
    {
        activatedUpgradeActionList = new List<AbstractUpgradeAction>()
        {
            new BackClick(),
            new CellClick(),
            new EmptyCellClick()
        };
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);


    }

    public override bool CanBeActive(SteamcopterCellInfo cellInfo)
    {
        if (cellInfo.superstructureType != HexagonSuperstructureType.none)
        {
            return true;
        }
        return false;
    }
}

public class BackClick : AbstractUpgradeAction
{
    public BackClick()
    {
        infoPanelData = new InfoPanelData("");
        actionID = UpgradeActionID.backClick;
        activatedNewUpgradeActionList = false;
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);

        if (upgradeManager.historyUpgradeButtonScript.Count >= 3)
        {
            AbstractUpgradeButtonScript aubs = upgradeManager.historyUpgradeButtonScript[upgradeManager.historyUpgradeButtonScript.Count - 3];
            AbstractUpgradeAction aua = upgradeManager.historyUpgradeAction[upgradeManager.historyUpgradeAction.Count - 3];

            //foreach (AbstractUpgradeAction aua1 in upgradeManager.historyUpgradeAction)
            //{
            //    Debug.Log(aua1.actionID);
            //}
            //if ((upgradeManager.historyUpgradeAction[upgradeManager.historyUpgradeAction.Count - 1].actionID == UpgradeActionID.protectionCategory_SelectArmorPlateClick) && (PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber).superstructureType == HexagonSuperstructureType.armorPlate))
            //{
            //    Debug.Log("log");
            //}
            if ((aua.actionID != UpgradeActionID.emptyCellClick) &&
                !((upgradeManager.historyUpgradeAction[upgradeManager.historyUpgradeAction.Count - 2].actionID == UpgradeActionID.protectionCategory_ArmorPlateClick) && (PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber).superstructureType == HexagonSuperstructureType.armorPlate)))
            {
                upgradeManager.RemoveLastHistoryEvent();
                upgradeManager.RemoveLastHistoryEvent();
                upgradeManager.RemoveLastHistoryEvent();

                upgradeManager.activeUpgradeActionList.Add(aua);
                upgradeManager.UpgradeButtonClick(aubs);
            }
            else
            {
                upgradeManager.ActivateStartActiveUpgradeAction();

            }
        }
        else
        {
            upgradeManager.ActivateStartActiveUpgradeAction();
        }

        upgradeManager.upgradeUserInterface.RemoveUpgradeArmorPlate();
    }
}

public class WeaponCategory_smallGunClick : AbstractUpgradeAction
{
    public WeaponCategory_smallGunClick()
    {
        cost = 15;
        infoPanelData = new InfoPanelData("Auto-Turret ''Needle'' - " + cost + " units" + "\nDamage: 4 hp     Reload: 1 sec");
        actionID = UpgradeActionID.weaponCategory_smallGunClick;
        activatedNewUpgradeActionList = false;
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);

        if (ConfirmationMode(upgradeButtonScript))
        {
            if (PlayerInfoManager.resources >= cost)
            {
                activatedNewUpgradeActionList = true;
                SteamcopterCellInfo sci = PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber);
                sci.superstructureType = HexagonSuperstructureType.autoTurret;
                sci.superstructure = HexagonSuperstructure.autoNeedle;
                PlayerInfoManager.UpdateSteamcopterCell(sci, cost);
                upgradeManager.upgradeUserInterface.UpdateUpgradeHexagonalCellScript(PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber));
            }

            else
            {
				infoPanelData = new InfoPanelData("not enough resources");
            }
        }

    }
}

public class WeaponCategory_bigGunClick : AbstractUpgradeAction
{
    public WeaponCategory_bigGunClick()
    {
        cost = 15;
        infoPanelData = new InfoPanelData("Turret ''Aloha'' - " + cost + " units" + "\nDamage: 3 hp    Shots: 6    Reload: 4 sec");
        actionID = UpgradeActionID.weaponCategory_bigGunClick;
        activatedNewUpgradeActionList = false;
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);

        if (ConfirmationMode(upgradeButtonScript))
        {
            if (PlayerInfoManager.resources >= cost)
            {
                activatedNewUpgradeActionList = true;
                SteamcopterCellInfo sci = PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber);
                sci.superstructureType = HexagonSuperstructureType.gun;
                sci.superstructure = HexagonSuperstructure.gunShestoper;
                PlayerInfoManager.UpdateSteamcopterCell(sci, cost);
                upgradeManager.upgradeUserInterface.UpdateUpgradeHexagonalCellScript(PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber));
            }
            else
            {
                infoPanelData = new InfoPanelData("not enough resources");
            }
        }
    }
}

public class SellClick : AbstractUpgradeAction
{
    public SellClick()
    {
        cost = -10;
        infoPanelData = new InfoPanelData("Sell the module for half of its cost?");
        actionID = UpgradeActionID.sellClick;
        activatedNewUpgradeActionList = false;
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);

        if ((PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber).superstructureType != HexagonSuperstructureType.none))
        {
            infoPanelData = new InfoPanelData("Sell the equipment for half of its cost?");
        }

        if (ConfirmationMode(upgradeButtonScript))
        {
            activatedNewUpgradeActionList = true;
            SteamcopterCellInfo sci = PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber);
            if (sci.superstructureType != HexagonSuperstructureType.none)
            {
                if (sci.superstructureType != HexagonSuperstructureType.armorPlate)
                {
                    PlayerInfoManager.UpdateSteamcopterCell(new SteamcopterCellInfo(sci.cellNumber, HexagonSuperstructureType.none, HexagonSuperstructure.none, new List<int>()), cost);
                    upgradeManager.upgradeUserInterface.UpdateUpgradeHexagonalCellScript(PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber));
                }

                if (sci.superstructureType == HexagonSuperstructureType.armorPlate)
                {
                    PlayerInfoManager.RemoveSteamcopterArmorPlate(PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber), (int)(-cost * 0.5f));
                    upgradeManager.upgradeUserInterface.UpdateUpgradeHexagonalCellScript(PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber));
                    upgradeManager.upgradeUserInterface.UpdateNeighborEmptyUpgradeHexagonalCell(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo);
                }

            }
            else
            {
                PlayerInfoManager.RemoveHexagonalCell(sci.cellNumber);
                upgradeManager.upgradeUserInterface.RemoveHexagonalCell(sci);
                //upgradeManager.ActivateStartActiveUpgradeAction();
            }
        }
    }

    public override bool CanBeActive(SteamcopterCellInfo cellInfo)
    {
        bool flag = false;
        if (cellInfo.superstructureType != HexagonSuperstructureType.commandCell)
        {
            flag = true;
        }

        if ((cellInfo.superstructureType == HexagonSuperstructureType.none) && (SteamcopterManager.Instance.GetFallsOffCells(cellInfo.cellNumber).Count > 0))
        {
            flag = false;
        }

        return flag;
    }
}

public class ProtectionCategory_SelectArmorPlateClick : AbstractUpgradeAction
{
    public UpgradeArmorPlateScript upgradeArmorPlateScript;
    public ProtectionCategory_SelectArmorPlateClick()
    {
        cost = 10;
        infoPanelData = new InfoPanelData("Armor plates - " + cost + " units" + "\nCost per plate: " + cost + "     Armor: 3");
        actionID = UpgradeActionID.protectionCategory_SelectArmorPlateClick;
        activatedNewUpgradeActionList = false;
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);
        upgradeManager.RemoveLastHistoryEvent();
        UpgradeArmorPlateScript uaps = (UpgradeArmorPlateScript)upgradeButtonScript;


        if (PlayerInfoManager.resources >= cost)
        {
            SteamcopterCellInfo sci = PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber);
            SteamcopterCellInfo _sci = new SteamcopterCellInfo(sci.cellNumber, HexagonSuperstructureType.armorPlate, HexagonSuperstructure.none, new List<int>() { uaps.position });
            //_sci.armorPlatePositions.Add(uaps.position);
            PlayerInfoManager.AddSteamcopterArmorPlate(_sci, cost);
            upgradeManager.upgradeUserInterface.UpdateUpgradeHexagonalCellScript(PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber));
            //activatedNewUpgradeActionList = true;

            //upgradeManager.upgradeUserInterface.RemoveButtonHexagons();
            //upgradeManager.upgradeUserInterface.VisualizeButtonHexagons(PlayerInfoManager.GetSteamcopterConfiguration());
            //upgradeManager.SetActiveUpgradeHexagonalCellScript(upgradeManager.upgradeUserInterface.fullUpgradeHexagonalCellList.Find(item => (item.cellInfo.cellNumber.Equals(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber))));
            upgradeManager.upgradeUserInterface.UpdateNeighborEmptyUpgradeHexagonalCell(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo);

            upgradeManager.upgradeUserInterface.RemoveUpgradeArmorPlate();
            upgradeManager.upgradeUserInterface.VisualizeUpgradeArmorPlate(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo);
        }
        else
        {
            infoPanelData = new InfoPanelData("not enough resources");
        }

    }
}

public class EngineCategory_GyrorotatorClick : AbstractUpgradeAction
{
    public EngineCategory_GyrorotatorClick()
    {
        cost = 12;
        infoPanelData = new InfoPanelData("Gyro-rotator - " + cost + " units" + "\nIncreases rotation speed by 30%");
        actionID = UpgradeActionID.engineCategory_GyrorotatorClick;
        activatedNewUpgradeActionList = false;
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);

        if (ConfirmationMode(upgradeButtonScript))
        {
            if (PlayerInfoManager.resources >= cost)
            {
                activatedNewUpgradeActionList = true;
                SteamcopterCellInfo sci = PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber);
                sci.superstructureType = HexagonSuperstructureType.engine;
                sci.superstructure = HexagonSuperstructure.engineGyrorotator;
                PlayerInfoManager.UpdateSteamcopterCell(sci, cost);
                upgradeManager.upgradeUserInterface.UpdateUpgradeHexagonalCellScript(PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber));

            }
            else
            {
				infoPanelData = new InfoPanelData("not enough resources");
            }
        }
    }
}

public class EngineCategory_TurbineClick : AbstractUpgradeAction
{
    public EngineCategory_TurbineClick()
    {
        cost = 10;
        infoPanelData = new InfoPanelData("Turbine - " + cost + " units" + "\nIncreases linear speed by 30%");
        actionID = UpgradeActionID.engineCategory_TurbineClick;
        activatedNewUpgradeActionList = false;
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);

        if (ConfirmationMode(upgradeButtonScript))
        {
            if (PlayerInfoManager.resources >= cost)
            {
                activatedNewUpgradeActionList = true;
                SteamcopterCellInfo sci = PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber);
                sci.superstructureType = HexagonSuperstructureType.engine;
                sci.superstructure = HexagonSuperstructure.engineTurbine;
                PlayerInfoManager.UpdateSteamcopterCell(sci, cost);
                upgradeManager.upgradeUserInterface.UpdateUpgradeHexagonalCellScript(PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber));

            }
            else
            {
				infoPanelData = new InfoPanelData("not enough resources");
            }
        }
    }
}

public class ProtectionCategory_ArmorPlateClick : AbstractUpgradeAction
{
    public ProtectionCategory_ArmorPlateClick()
    {
		infoPanelData = new InfoPanelData("Add armor plates in red spots for 10 units each");
        actionID = UpgradeActionID.protectionCategory_ArmorPlateClick;
        activatedNewUpgradeActionList = true;
    }

    public override void CreateActivatedUpgradeActionList()
    {
        activatedUpgradeActionList = new List<AbstractUpgradeAction>()
        {
            new ProtectionCategory_SelectArmorPlateClick(),
            new BackClick()
        };
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);

        upgradeManager.upgradeUserInterface.VisualizeUpgradeArmorPlate(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo);
    }

    public override bool CanBeActive(SteamcopterCellInfo cellInfo)
    {
        if (cellInfo.superstructureType == HexagonSuperstructureType.none)
        {
            return true;
        }
        return false;
    }
}

public class ProtectionCategory_PointDefenseClick : AbstractUpgradeAction
{
    public ProtectionCategory_PointDefenseClick()
    {
        cost = 10;
        infoPanelData = new InfoPanelData("Point Defense - " + cost + " units" + "\nShoots 1 missile every 6 seconds");
        actionID = UpgradeActionID.protectionCategory_PointDefenseClick;
        activatedNewUpgradeActionList = false;
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);

        if (ConfirmationMode(upgradeButtonScript))
        {
            if (PlayerInfoManager.resources >= cost)
            {
                activatedNewUpgradeActionList = true;
                SteamcopterCellInfo sci = PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber);
                sci.superstructureType = HexagonSuperstructureType.autoTurret;
                sci.superstructure = HexagonSuperstructure.autoPointDefense;
                PlayerInfoManager.UpdateSteamcopterCell(sci, cost);
                upgradeManager.upgradeUserInterface.UpdateUpgradeHexagonalCellScript(PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber));
            }
            else
            {
				infoPanelData = new InfoPanelData("not enough resources");
            }

        }
    }

    public override bool CanBeActive(SteamcopterCellInfo cellInfo)
    {
        if (cellInfo.superstructureType == HexagonSuperstructureType.none)
        {
            return true;
        }
        return false;
    }
}

public class WeaponCategory_RocketProtuberanetsClick : AbstractUpgradeAction
{
    public WeaponCategory_RocketProtuberanetsClick()
    {
        cost = 30;
        infoPanelData = new InfoPanelData("Launcher ''Protuberance'' - " + cost + " units" + "\nDamage: 30 hp    Shots: 2     Reload: 8 sec");
        actionID = UpgradeActionID.weaponCategory_RocketProtuberanetsClick;
        activatedNewUpgradeActionList = false;
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);

        if (ConfirmationMode(upgradeButtonScript))
        {
            if (PlayerInfoManager.resources >= cost)
            {
                activatedNewUpgradeActionList = true;
                SteamcopterCellInfo sci = PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber);
                sci.superstructureType = HexagonSuperstructureType.rocket;
                sci.superstructure = HexagonSuperstructure.rocketProtuberanets;
                PlayerInfoManager.UpdateSteamcopterCell(sci, cost);
                upgradeManager.upgradeUserInterface.UpdateUpgradeHexagonalCellScript(PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber));

            }
            else
            {
				infoPanelData = new InfoPanelData("not enough resources");
            }
        }
    }
}

public class WeaponCategory_JawsClick : AbstractUpgradeAction
{
    public WeaponCategory_JawsClick()
    {
        cost = 20;
        infoPanelData = new InfoPanelData("Melee weapon ''Jaws'' - " + cost + " units" + "\nDamage: 25 hp    Reload: 2 sec");
        actionID = UpgradeActionID.weaponCategory_JawsClick;
        activatedNewUpgradeActionList = false;
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);

        if (ConfirmationMode(upgradeButtonScript))
        {
            if (PlayerInfoManager.resources >= cost)
            {
                activatedNewUpgradeActionList = true;
                SteamcopterCellInfo sci = PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber);
                sci.superstructureType = HexagonSuperstructureType.autoTurret;
                sci.superstructure = HexagonSuperstructure.autoJaws;
                PlayerInfoManager.UpdateSteamcopterCell(sci, cost);
                upgradeManager.upgradeUserInterface.UpdateUpgradeHexagonalCellScript(PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber));
            }
            else
            {
				infoPanelData = new InfoPanelData("not enough resources");
            }
        }
    }
}

public class SuperEquipmentCategoryClick : AbstractUpgradeAction
{
    public SuperEquipmentCategoryClick()
    {
        infoPanelData = new InfoPanelData("select improvement");
        actionID = UpgradeActionID.superEquipmentCategoryClick;
        activatedNewUpgradeActionList = true;
    }

    public override void CreateActivatedUpgradeActionList()
    {
        activatedUpgradeActionList = new List<AbstractUpgradeAction>()
        {
            new SuperEquipmentCategory_MagnetosphereClick(),
            new CellClick(),
            new BackClick()
        };
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);

    }

    public override bool CanBeActive(SteamcopterCellInfo cellInfo)
    {
        if ((cellInfo.superstructureType == HexagonSuperstructureType.none) && 
            (SteamcopterManager.Instance.superEquipment == null))
        {
            return true;
        }
        return false;
    }
}

public class SuperEquipmentCategory_MagnetosphereClick : AbstractUpgradeAction
{
    public SuperEquipmentCategory_MagnetosphereClick()
    {
        cost = 20;
        infoPanelData = new InfoPanelData("Magneto-sphere - " + cost + " units" + "\nReverses all incoming shells.      Reload: 13 sec");
        actionID = UpgradeActionID.superEquipmentCategory_MagnetosphereClick;
        activatedNewUpgradeActionList = false;
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);

        if (ConfirmationMode(upgradeButtonScript))
        {
            if (PlayerInfoManager.resources >= cost)
            {
                activatedNewUpgradeActionList = true;
                SteamcopterCellInfo sci = PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber);
                sci.superstructureType = HexagonSuperstructureType.superEquipment;
                sci.superstructure = HexagonSuperstructure.superMagnetosphere;
                PlayerInfoManager.UpdateSteamcopterCell(sci, cost);
                upgradeManager.upgradeUserInterface.UpdateUpgradeHexagonalCellScript(PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber));
            }
            else
            {
				infoPanelData = new InfoPanelData("not enough resources");
            }

        }
    }

    public override bool CanBeActive(SteamcopterCellInfo cellInfo)
    {
        if ((cellInfo.superstructureType == HexagonSuperstructureType.none) &&
            (SteamcopterManager.Instance.superEquipment == null))
        {
            return true;
        }
        return false;
    }
}

public class ProtectionCategory_SuppressorClick : AbstractUpgradeAction
{
    public ProtectionCategory_SuppressorClick()
    {
        cost = 20;
        infoPanelData = new InfoPanelData("''Suppressor'' - " + cost + " units" + "\nDisables random enemy for 6 seconds every 10 seconds");
        actionID = UpgradeActionID.protectionCategory_SuppressorClick;
        activatedNewUpgradeActionList = false;
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);

        if (ConfirmationMode(upgradeButtonScript))
        {
            if (PlayerInfoManager.resources >= cost)
            {
                activatedNewUpgradeActionList = true;
                SteamcopterCellInfo sci = PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber);
                sci.superstructureType = HexagonSuperstructureType.autoTurret;
                sci.superstructure = HexagonSuperstructure.autoSuppressor;
                PlayerInfoManager.UpdateSteamcopterCell(sci, cost);
                upgradeManager.upgradeUserInterface.UpdateUpgradeHexagonalCellScript(PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber));
            }
            else
            {
				infoPanelData = new InfoPanelData("not enough resources");
            }
        }
    }

    public override bool CanBeActive(SteamcopterCellInfo cellInfo)
    {
        if (cellInfo.superstructureType == HexagonSuperstructureType.none)
        {
            return true;
        }
        return false;
    }
}

public class WeaponCategory_RhinoClick : AbstractUpgradeAction
{
    public WeaponCategory_RhinoClick()
    {
        cost = 12;
        infoPanelData = new InfoPanelData("Turret ''Rhino'' - " + cost + " units" + "\nDamage: 12 hp     Reload: 3 sec ");
        actionID = UpgradeActionID.weaponCategory_RhinoClick;
        activatedNewUpgradeActionList = false;
    }

    public override void Activate(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        base.Activate(upgradeButtonScript);

        if (ConfirmationMode(upgradeButtonScript))
        {
            if (PlayerInfoManager.resources >= cost)
            {
                activatedNewUpgradeActionList = true;
                SteamcopterCellInfo sci = PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber);
                sci.superstructureType = HexagonSuperstructureType.gun;
                sci.superstructure = HexagonSuperstructure.gunRhino;
                PlayerInfoManager.UpdateSteamcopterCell(sci, cost);
                upgradeManager.upgradeUserInterface.UpdateUpgradeHexagonalCellScript(PlayerInfoManager.GetSteamcopterCellInfo(upgradeManager.GetActiveUpgradeHexagonalCellScript().cellInfo.cellNumber));
            }
            else
            {
				infoPanelData = new InfoPanelData("not enough resources");
            }
        }
    }
}