using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpgradeUserInterface : MonoBehaviour {

    public InputManager inputManager;
    public UpgradeManager upgradeManager;
    public GameObject upgradeHexagonalCell;
    public GameObject upgradeArmorPlate;
    public GameObject content;
    public tk2dTextMesh infoPanel;
    public tk2dTextMesh resourcesPanel;
    public tk2dUIItem returnToGameButton;

    public tk2dUIScrollableArea scrollableArea;
    public GameObject scrollableAreaGameObject;

    public ControllerNumericCounter controllerNumericCounter;

    public List<UpgradeHexagonalCellScript> fullUpgradeHexagonalCellList = new List<UpgradeHexagonalCellScript>();
    public List<UpgradeHexagonalCellScript> emptyUpgradeHexagonalCellList = new List<UpgradeHexagonalCellScript>();
    public List<UpgradeArmorPlateScript> upgradeArmorPlateScriptList = new List<UpgradeArmorPlateScript>();

    void Awake()
    {
        returnToGameButton.OnClickUIItem += GameButtonPressed;
    }

    public void UpdateGUI()
    {
        resourcesPanel.text = PlayerInfoManager.resources.ToString();
        resourcesPanel.Commit();
        controllerNumericCounter.UpdateNumericCounter(PlayerInfoManager.resources);
    }

    public void SetInfoPanelText(InfoPanelData infoPanelData)
    {
        infoPanel.text = infoPanelData.infoPanelText;
        infoPanel.Commit();
    }

    public void AddButtonToScrollableArea(List<tk2dUILayout> tk2dUILayoutList)
    {
        scrollableAreaGameObject.SetActive(true);
        List<tk2dUILayoutItem> list = new List<tk2dUILayoutItem>();
        list.AddRange(scrollableArea.ContentLayoutContainer.layoutItems);
        foreach (tk2dUILayoutItem l in list)
        {
            scrollableArea.ContentLayoutContainer.RemoveLayout(l.layout);
            Destroy(l.gameObj);
        }
        foreach (tk2dUILayout layout in tk2dUILayoutList)
        {
            scrollableArea.ContentLayoutContainer.AddLayout(layout, tk2dUILayoutItem.FixedSizeLayoutItem());
            layout.gameObject.SetActive(true);
        }

        scrollableArea.ContentLayoutContainer.Refresh();

        if (tk2dUILayoutList.Count == 0)
        {
            scrollableAreaGameObject.SetActive(false);
        }
    }

    public void VisualizeUpgradeArmorPlate(SteamcopterCellInfo cellInfo)
    {
        List<CellNumber> neighborCellsList = cellInfo.cellNumber.GetAllNeighborCells();
        neighborCellsList.Add(neighborCellsList[0]);

        List<int> positionList = new List<int>();

        for (int i = 0; i < 6; i++)
        {
            if ((!fullUpgradeHexagonalCellList.Exists(item => (item.cellInfo.cellNumber.Equals(neighborCellsList[i])))) &&
                (!fullUpgradeHexagonalCellList.Exists(item => (item.cellInfo.cellNumber.Equals(neighborCellsList[i+1])))) &&
                (!cellInfo.armorPlatePositions.Exists(item => (((i + 2) % 6)==item))))
            {
                positionList.Add((i + 2) % 6);
            }
        }

        float Angle = (SteamcopterManager.Instance.gameObject.transform.rotation.eulerAngles.y / 180) * Mathf.PI;
        foreach (int pos in positionList)
        {
            Vector3 position = CellNumber.RotateVector(CellNumber.GetCellPosition(cellInfo.cellNumber) - SteamcopterManager.Instance.cellPositionOffset, Angle)
                + SteamcopterManager.Instance.gameObject.transform.position;
            GameObject go = (GameObject)Instantiate(upgradeArmorPlate, position, SteamcopterManager.Instance.gameObject.transform.rotation);
            go.transform.position = go.transform.position + new Vector3(0, 0.2f, 0);
            go.transform.Rotate(new Vector3(90, 0, 0));
            go.transform.Rotate(new Vector3(0, 0, pos * -60));
            UpgradeArmorPlateScript uaps = go.GetComponent<UpgradeArmorPlateScript>();
            uaps.upgradeManager = upgradeManager;
            uaps.cellInfo = new SteamcopterCellInfo(cellInfo.cellNumber, HexagonSuperstructureType.none, HexagonSuperstructure.none, null);
            uaps.upgradeActionID = UpgradeActionID.protectionCategory_SelectArmorPlateClick;
            uaps.position = pos;
            upgradeArmorPlateScriptList.Add(uaps);
            go.transform.parent = content.transform;
        }
    }

    public void RemoveUpgradeArmorPlate()
    {
        for (int i = (upgradeArmorPlateScriptList.Count - 1); i >= 0; i--)
        {
            Destroy(upgradeArmorPlateScriptList[i].gameObject);
        }

        upgradeArmorPlateScriptList = new List<UpgradeArmorPlateScript>();
    }

    public void UpdateNeighborEmptyUpgradeHexagonalCell(SteamcopterCellInfo cellInfo)
    {
        List<CellNumber> list = cellInfo.cellNumber.GetNeighborCells();
        foreach (CellNumber cn in list)
        {
            if ((emptyUpgradeHexagonalCellList.Exists(item => (item.cellInfo.cellNumber.Equals(cn)))))
            {
                int k = emptyUpgradeHexagonalCellList.FindIndex(item => (item.cellInfo.cellNumber.Equals(cn)));
                Destroy(emptyUpgradeHexagonalCellList[k].gameObject);
                emptyUpgradeHexagonalCellList.RemoveAt(k);
            }
        }

        for (int j = list.Count - 1; j >= 0; j--)
        {
            CellNumber cn = list[j];
            List<CellNumber> listAll = cn.GetAllNeighborCells();
            for (int i = listAll.Count - 1; i >= 0; i--)
            {
                UpgradeHexagonalCellScript uhcs = fullUpgradeHexagonalCellList.Find(item => ((item.cellInfo.cellNumber.x == listAll[i].x) && (item.cellInfo.cellNumber.y == listAll[i].y)));
                if ((uhcs != null) && ((uhcs.cellInfo.armorPlatePositions.Exists(item => item == ((i + 1 + 3) % 6))) || (uhcs.cellInfo.armorPlatePositions.Exists(item => item == ((i + 2 + 3) % 6)))))
                {
                    list.RemoveAt(j);
                    break;
                }
            }
        }

        foreach (CellNumber cn in list)
        {
            if ((!fullUpgradeHexagonalCellList.Exists(item => (item.cellInfo.cellNumber.Equals(cn)))) &&
                (!emptyUpgradeHexagonalCellList.Exists(item => (item.cellInfo.cellNumber.Equals(cn)))))
            {
                CreateEmptyUpgradeHexagonalCell(cn);
            }
        }
    }

    public void UpdateUpgradeHexagonalCellScript(SteamcopterCellInfo cellInfo)
    {
        UpgradeHexagonalCellScript uhcs = fullUpgradeHexagonalCellList.Find(item => (item.cellInfo.cellNumber.Equals(cellInfo.cellNumber)));
        AudioSource.PlayClipAtPoint(BaseOfSounds.Instance.constructionOfSuperstructure, uhcs.gameObject.transform.position);
        if (uhcs == null)
        {
            Debug.LogError("UpgradeUserInterface::UpdateUpgradeHexagonalCellScript: Null reference");
        }
        uhcs.cellInfo = cellInfo;
    }

    public void RemoveHexagonalCell(SteamcopterCellInfo cellInfo)
    {
        if (!fullUpgradeHexagonalCellList.Exists(item => (item.cellInfo.cellNumber.Equals(cellInfo.cellNumber))))
        {
            Debug.LogError("UpgradeUserInterface::RemoveHexagonalCell: Null reference");
        }
        int n = fullUpgradeHexagonalCellList.FindIndex(item => (item.cellInfo.cellNumber.Equals(cellInfo.cellNumber)));
        Destroy(fullUpgradeHexagonalCellList[n].gameObject);
        fullUpgradeHexagonalCellList.RemoveAt(n);

        List<CellNumber> list = cellInfo.cellNumber.GetNeighborCells();
        foreach (CellNumber cn in list)
        {
            if ((emptyUpgradeHexagonalCellList.Exists(item => (item.cellInfo.cellNumber.Equals(cn)))))
            {
                int k = emptyUpgradeHexagonalCellList.FindIndex(item => (item.cellInfo.cellNumber.Equals(cn)));
                Destroy(emptyUpgradeHexagonalCellList[k].gameObject);
                emptyUpgradeHexagonalCellList.RemoveAt(k);
            }
        }

        List<CellNumber> list1 = new List<CellNumber>();
        foreach (CellNumber cn in list)
        {
            if ((fullUpgradeHexagonalCellList.Exists(item => (item.cellInfo.cellNumber.Equals(cn)))))
            {
                list1.AddRange(cn.GetNeighborCells());
            }
        }

        for (int j = list1.Count - 1; j >= 0; j--)
        {
            CellNumber cn = list1[j];
            List<CellNumber> listAll = cn.GetAllNeighborCells();
            for (int i = listAll.Count - 1; i >= 0; i--)
            {
                UpgradeHexagonalCellScript uhcs = fullUpgradeHexagonalCellList.Find(item => ((item.cellInfo.cellNumber.x == listAll[i].x) && (item.cellInfo.cellNumber.y == listAll[i].y)));
                if ((uhcs != null) && ((uhcs.cellInfo.armorPlatePositions.Exists(item => item == ((i + 1 + 3) % 6))) || (uhcs.cellInfo.armorPlatePositions.Exists(item => item == ((i + 2 + 3) % 6)))))
                {
                    list1.RemoveAt(j);
                    break;
                }
            }
        }

        foreach (CellNumber cn1 in list1)
        {
            if ((!fullUpgradeHexagonalCellList.Exists(item => (item.cellInfo.cellNumber.Equals(cn1)))) &&
                (!emptyUpgradeHexagonalCellList.Exists(item => (item.cellInfo.cellNumber.Equals(cn1)))))
            {
                CreateEmptyUpgradeHexagonalCell(cn1);
            }
        }
    }

    public void AddHexagonalCell(SteamcopterCellInfo cellInfo)
    {
        if (!emptyUpgradeHexagonalCellList.Exists(item => (item.cellInfo.cellNumber.Equals(cellInfo.cellNumber))))
        {
            Debug.LogError("UpgradeUserInterface::AddHexagonalCell: Null reference");
        }
        int n = emptyUpgradeHexagonalCellList.FindIndex(item => (item.cellInfo.cellNumber.Equals(cellInfo.cellNumber)));
        Destroy(emptyUpgradeHexagonalCellList[n].gameObject);
        emptyUpgradeHexagonalCellList.RemoveAt(n);
        CreateFullUpgradeHexagonalCell(cellInfo);

        List<CellNumber> list = cellInfo.cellNumber.GetNeighborCells();
        for (int j = list.Count - 1; j >= 0; j--)
        {
            CellNumber cn = list[j];
            List<CellNumber> listAll = cn.GetAllNeighborCells();
            for (int i = listAll.Count - 1; i >= 0; i--)
            {
                UpgradeHexagonalCellScript uhcs = fullUpgradeHexagonalCellList.Find(item => ((item.cellInfo.cellNumber.x == listAll[i].x) && (item.cellInfo.cellNumber.y == listAll[i].y)));
                if ((uhcs != null) && ((uhcs.cellInfo.armorPlatePositions.Exists(item => item == ((i + 1 + 3) % 6))) || (uhcs.cellInfo.armorPlatePositions.Exists(item => item == ((i + 2 + 3) % 6)))))
                {
                    list.RemoveAt(j);
                    break;
                }
            }
        }
        foreach (CellNumber cn in list)
        {
            if ((!fullUpgradeHexagonalCellList.Exists(item => (item.cellInfo.cellNumber.Equals(cn)))) &&
                (!emptyUpgradeHexagonalCellList.Exists(item => (item.cellInfo.cellNumber.Equals(cn)))))
            {
                CreateEmptyUpgradeHexagonalCell(cn);
            }
        }
    }

    public void CreateFullUpgradeHexagonalCell(SteamcopterCellInfo cellInfo)
    {
        CreateUpgradeHexagonalCell(cellInfo.cellNumber, cellInfo);
    }

    public void CreateEmptyUpgradeHexagonalCell(CellNumber cellNumber)
    {
        CreateUpgradeHexagonalCell(cellNumber, null);
    }

    private void CreateUpgradeHexagonalCell(CellNumber cellNumber, SteamcopterCellInfo cellInfo)
    {
        UpgradeActionID uaid = UpgradeActionID.cellClick;
        List<UpgradeHexagonalCellScript> upgradeHexagonalCellList = fullUpgradeHexagonalCellList;
        if (cellInfo == null)
        {
            cellInfo = new SteamcopterCellInfo(cellNumber, HexagonSuperstructureType.none, HexagonSuperstructure.none, new List<int>());
            uaid = UpgradeActionID.emptyCellClick;
            upgradeHexagonalCellList = emptyUpgradeHexagonalCellList;
        }

        float Angle = (SteamcopterManager.Instance.gameObject.transform.rotation.eulerAngles.y / 180) * Mathf.PI;
        Vector3 position = CellNumber.RotateVector(CellNumber.GetCellPosition(cellNumber) - SteamcopterManager.Instance.cellPositionOffset, Angle)
            + SteamcopterManager.Instance.gameObject.transform.position;
        GameObject go = (GameObject)Instantiate(upgradeHexagonalCell, position, SteamcopterManager.Instance.gameObject.transform.rotation);
        go.transform.Rotate(new Vector3(90, 0, 0));
        UpgradeHexagonalCellScript uhcs = go.GetComponent<UpgradeHexagonalCellScript>();
        uhcs.upgradeManager = upgradeManager;
        uhcs.cellInfo = cellInfo;
        uhcs.upgradeActionID = uaid;
        go.transform.parent = content.transform;
        upgradeHexagonalCellList.Add(uhcs);
    }

    public void RemoveButtonHexagons()
    {
        for (int i = (emptyUpgradeHexagonalCellList.Count - 1); i >= 0; i--)
        {
            Destroy(emptyUpgradeHexagonalCellList[i].gameObject);
        }
        emptyUpgradeHexagonalCellList = new List<UpgradeHexagonalCellScript>();

        for (int i = (fullUpgradeHexagonalCellList.Count - 1); i >= 0; i--)
        {
            Destroy(fullUpgradeHexagonalCellList[i].gameObject);
        }
        fullUpgradeHexagonalCellList = new List<UpgradeHexagonalCellScript>();
    }

    public void VisualizeButtonHexagons(List<SteamcopterCellInfo> CellInfoList)
    {
        foreach (SteamcopterCellInfo sci in CellInfoList)
        {
            CreateFullUpgradeHexagonalCell(sci);
        }

        List<CellNumber> cellNumberList = new List<CellNumber>();
        foreach (SteamcopterCellInfo sci in CellInfoList)
        {
            List<CellNumber> listAll = sci.cellNumber.GetAllNeighborCells();
            List<CellNumber> list = sci.cellNumber.GetNeighborCells();

            foreach (CellNumber cn in list)
            {
                if (cellNumberList.Exists(item => ((item.x == cn.x) && (item.y == cn.y))) == false)
                {
                    cellNumberList.Add(cn);
                }
            }
        }
        foreach (SteamcopterCellInfo sci in CellInfoList)
        {
            cellNumberList.RemoveAll(item => ((item.x == sci.cellNumber.x) && (item.y == sci.cellNumber.y)));
        }

        for (int j = cellNumberList.Count - 1; j >= 0; j-- )
        {
            CellNumber cn = cellNumberList[j];
            List<CellNumber> listAll = cn.GetAllNeighborCells();
            for (int i = listAll.Count - 1; i >= 0; i--)
            {
                SteamcopterCellInfo sci = CellInfoList.Find(item => ((item.cellNumber.x == listAll[i].x) && (item.cellNumber.y == listAll[i].y)));
                if ((sci != null) && ((sci.armorPlatePositions.Exists(item => item == ((i + 1 + 3) % 6))) || (sci.armorPlatePositions.Exists(item => item == ((i + 2 + 3) % 6)))))
                {
                    cellNumberList.RemoveAt(j);
                    break;
                }
            }
        }

        foreach (CellNumber cn in cellNumberList)
        {
            CreateEmptyUpgradeHexagonalCell(cn);
        }
    }

    void OnEnable()
    {
        VisualizeButtonHexagons(PlayerInfoManager.GetSteamcopterConfiguration());
        UpdateGUI();
    }

    void GameButtonPressed(tk2dUIItem Tk2dUIItem)
    {
        RemoveButtonHexagons();
        inputManager.ActivateGameInterface();
        TrainingScript.Instance.ReturnedToGame();
    }
}
