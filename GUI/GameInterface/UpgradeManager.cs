using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public UpgradeUserInterface upgradeUserInterface;
    public tk2dUILayout upgradeButtonPrefab;
    public List<AbstractUpgradeAction> activeUpgradeActionList;

    public List<AbstractUpgradeButtonScript> historyUpgradeButtonScript = new List<AbstractUpgradeButtonScript>();
    public List<AbstractUpgradeAction> historyUpgradeAction = new List<AbstractUpgradeAction>();

    private UpgradeHexagonalCellScript activeUpgradeHexagonalCellScript = null;
    public UpgradeButtonScript lastActiveUpgradeButtonScript = null;

    public void SaveAtHistory(AbstractUpgradeButtonScript upgradeButtonScript, AbstractUpgradeAction upgradeAction)
    {
        historyUpgradeButtonScript.Add(upgradeButtonScript);
        historyUpgradeAction.Add(upgradeAction);
        if (historyUpgradeButtonScript.Count > 20)
        {
            historyUpgradeButtonScript.RemoveAt(0);
            historyUpgradeAction.RemoveAt(0);
        }
    }

    public void RemoveLastHistoryEvent()
    {
        historyUpgradeButtonScript.RemoveAt(historyUpgradeButtonScript.Count - 1);
        historyUpgradeAction.RemoveAt(historyUpgradeAction.Count - 1);
    }

    public void ActivateStartActiveUpgradeAction()
    {
        activeUpgradeActionList = new List<AbstractUpgradeAction>();
        activeUpgradeActionList.Add(new EmptyCellClick());
        activeUpgradeActionList.Add(new CellClick());
        SetActiveUpgradeHexagonalCellScript(null);
        upgradeUserInterface.SetInfoPanelText(new InfoPanelData("Select the cell to add new module or sell existing one"));
        historyUpgradeButtonScript = new List<AbstractUpgradeButtonScript>();
        historyUpgradeAction = new List<AbstractUpgradeAction>();
        upgradeUserInterface.AddButtonToScrollableArea(new List<tk2dUILayout>());

    }

    public UpgradeHexagonalCellScript GetActiveUpgradeHexagonalCellScript()
    {
        return activeUpgradeHexagonalCellScript;
    }

    public void SetActiveUpgradeHexagonalCellScript(UpgradeHexagonalCellScript upgradeHexagonalCellScript)
    {
        if (activeUpgradeHexagonalCellScript != null)
        {
            if (upgradeHexagonalCellScript != null)
            {
                if (activeUpgradeHexagonalCellScript != upgradeHexagonalCellScript)
                {
                    activeUpgradeHexagonalCellScript.DeactivateCheckSprite();
                }
            }
            else
            {
                activeUpgradeHexagonalCellScript.DeactivateCheckSprite();
            }
        }
        activeUpgradeHexagonalCellScript = upgradeHexagonalCellScript;
    }

    public void UpgradeButtonClick(AbstractUpgradeButtonScript upgradeButtonScript)
    {
        AbstractUpgradeAction aua = activeUpgradeActionList.Find(item => (item.actionID == upgradeButtonScript.upgradeActionID));
        if (aua != null)
        {
            SaveAtHistory(upgradeButtonScript, aua);
            aua.Activate(upgradeButtonScript);
            upgradeUserInterface.SetInfoPanelText(aua.infoPanelData);
            if (aua.activatedNewUpgradeActionList)
            {
                activeUpgradeActionList = new List<AbstractUpgradeAction>();
                upgradeUserInterface.AddButtonToScrollableArea(CreateUpgradeButtonList(aua.GetActivatedUpgradeActionList()));
            }
        }
        upgradeUserInterface.UpdateGUI();
    }

    private List<tk2dUILayout> CreateUpgradeButtonList(List<AbstractUpgradeAction> upgradeActionList)
    {
        List<tk2dUILayout> list = new List<tk2dUILayout>();
        if (upgradeActionList != null)
        {
            foreach (AbstractUpgradeAction aua in upgradeActionList)
            {
                if (aua.CanBeActive(activeUpgradeHexagonalCellScript.cellInfo))
                {
                    activeUpgradeActionList.Add(aua);
                    tk2dUILayout layout = CreateUpgradeButton(aua);
                    if (layout != null)
                    {
                        list.Add(layout);
                    }
                }
            }
        }
        if (list.Count == 0)
        {
            ActivateStartActiveUpgradeAction();
        }
        return list;
    }

    private tk2dUILayout CreateUpgradeButton(AbstractUpgradeAction upgradeAction)
    {
        switch (upgradeAction.actionID)
        {
            case UpgradeActionID.emptyCellClick:
                return null;
            case UpgradeActionID.cellClick:
                return null;
            case UpgradeActionID.protectionCategory_SelectArmorPlateClick:
                return null;
        }

        tk2dUILayout layout = Instantiate(upgradeButtonPrefab) as tk2dUILayout;
        UpgradeButtonScript ubs = layout.GetComponent<UpgradeButtonScript>();
        ubs.upgradeActionID = upgradeAction.actionID;
        ubs.upgradeManager = this;
        switch (upgradeAction.actionID)
        {
            case UpgradeActionID.engineCategoryClick:
                ubs.spriteID = 1;
                ubs.text = "Engines";
                break;
            case UpgradeActionID.weaponCategoryClick:
                ubs.spriteID = 2;
                ubs.text = "Weapons";
                break;
            case UpgradeActionID.protectionCategoryClick:
                ubs.spriteID = 3;
                ubs.text = "Defense";
                break;
            case UpgradeActionID.upgradeCategoryClick:
                ubs.spriteID = 4;
                ubs.text = "Confirm";
                break;
            case UpgradeActionID.backClick:
                ubs.spriteID = 5;
                break;
            case UpgradeActionID.weaponCategory_smallGunClick:
                ubs.spriteID = 6;
                break;
            case UpgradeActionID.weaponCategory_bigGunClick:
                ubs.spriteID = 7;
                break;
            case UpgradeActionID.sellClick:
                ubs.spriteID = 8;
                ubs.text = "Sell";
                break;
            case UpgradeActionID.engineCategory_GyrorotatorClick:
                ubs.spriteID = 9;
                break;
            case UpgradeActionID.engineCategory_TurbineClick:
                ubs.spriteID = 10;
                break;
            case UpgradeActionID.protectionCategory_PointDefenseClick:
                ubs.spriteID = 11;
                break;
            case UpgradeActionID.protectionCategory_ArmorPlateClick:
                ubs.spriteID = 12;
                break;
            case UpgradeActionID.weaponCategory_RocketProtuberanetsClick:
                ubs.spriteID = 13;
                break;
            case UpgradeActionID.weaponCategory_JawsClick:
                ubs.spriteID = 14;
                break;
            case UpgradeActionID.superEquipmentCategoryClick:
                ubs.spriteID = 15;
                break;
            case UpgradeActionID.superEquipmentCategory_MagnetosphereClick:
                ubs.spriteID = 16;
                break;
            case UpgradeActionID.protectionCategory_SuppressorClick:
                ubs.spriteID = 17;
                break;
            case UpgradeActionID.weaponCategory_RhinoClick:
                ubs.spriteID = 18;
                break;
        }
        return layout;
    }

    void OnEnable()
    {
        ActivateStartActiveUpgradeAction();
        AbstractUpgradeAction.upgradeManager = this;
    }
}
