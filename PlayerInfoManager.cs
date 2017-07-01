using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class PlayerInfoManager
{
    public static int resources = 300;
    public static int resourcesReceived = 0;
    public static int KilledEnemies = 0;
    private static List<SteamcopterCellInfo> steamcopterConfiguration;
    private static int startHexagonalCellCost = 8;
    private static int hexagonalCellCost = -1;

    public static void LoadPlayerInfo()
    {
        resources = 300;
        resourcesReceived = 0;
        KilledEnemies = 0;
        startHexagonalCellCost = 10;
        hexagonalCellCost = -1;

        List<SteamcopterCellInfo> list = new List<SteamcopterCellInfo>();

        list.Add(new SteamcopterCellInfo(new CellNumber(9, 7), HexagonSuperstructureType.superEquipment, HexagonSuperstructure.superMagnetosphere, new List<int>() {}));
        list.Add(new SteamcopterCellInfo(new CellNumber(9, 8), HexagonSuperstructureType.commandCell, HexagonSuperstructure.none, new List<int>() { }));
        list.Add(new SteamcopterCellInfo(new CellNumber(10, 8), HexagonSuperstructureType.commandCell, HexagonSuperstructure.none, new List<int>() { }));
        list.Add(new SteamcopterCellInfo(new CellNumber(8, 9), HexagonSuperstructureType.gun, HexagonSuperstructure.gunRhino, new List<int>() { }));
        list.Add(new SteamcopterCellInfo(new CellNumber(9, 9), HexagonSuperstructureType.commandCell, HexagonSuperstructure.none, new List<int>() { }));
        list.Add(new SteamcopterCellInfo(new CellNumber(10, 9), HexagonSuperstructureType.autoTurret, HexagonSuperstructure.autoSuppressor, new List<int>() { }));

        steamcopterConfiguration = list;
    }

    private static void CalculateHexagonalCellCost()
    {
        hexagonalCellCost = Mathf.RoundToInt(startHexagonalCellCost * Mathf.Pow(1.25f, GetSteamcopterConfiguration().Count - 1));
    }

    public static List<SteamcopterCellInfo> GetStartSteamcopterConfiguration()
    {
        List<SteamcopterCellInfo> list = new List<SteamcopterCellInfo>();
        list.AddRange(steamcopterConfiguration);
        return list;
    }

    public static List<SteamcopterCellInfo> GetSteamcopterConfiguration()
    {
        List<SteamcopterCellInfo> list = new List<SteamcopterCellInfo>();

        for (int i = 0; i < SteamcopterManager.steamcopterCellArrayAmount; i++)
            for (int j = 0; j < SteamcopterManager.steamcopterCellArrayAmount; j++)
            {
                SteamcopterCellInfo sci = new SteamcopterCellInfo(new CellNumber(i, j), HexagonSuperstructureType.none, HexagonSuperstructure.none, new List<int>());
                SteamcopterHexagonScript shs = SteamcopterManager.Instance.steamcopterCellArray[i, j];
                if (shs != null)
                {
                    if (shs is SteamcopterCommandCellScript)
                    {
                        sci.superstructureType = HexagonSuperstructureType.commandCell;
                    }
                    if (shs.superstructure != null)
                    {
                        if (shs.superstructure is AutoTurretAbstract)
                        {
                            sci.superstructureType = HexagonSuperstructureType.autoTurret;
                            if (shs.superstructure is AutoNeedleScript)
                                sci.superstructure = HexagonSuperstructure.autoNeedle;
                            if (shs.superstructure is AutoJawsScript)
                                sci.superstructure = HexagonSuperstructure.autoJaws;
                            if (shs.superstructure is AutoPointDefenseScript)
                                sci.superstructure = HexagonSuperstructure.autoPointDefense;
                            if (shs.superstructure is AutoSuppressorScript)
                                sci.superstructure = HexagonSuperstructure.autoSuppressor;
                        }
                        if (shs.superstructure is BigGunAbstract)
                        {
                            sci.superstructureType = HexagonSuperstructureType.gun;
                            if (shs.superstructure is GunShestoperScript)
                            sci.superstructure = HexagonSuperstructure.gunShestoper;
                            if (shs.superstructure is GunRhinoScript)
                                sci.superstructure = HexagonSuperstructure.gunRhino;
                        }
                        if (shs.superstructure is RocketAbstract)
                        {
                            sci.superstructureType = HexagonSuperstructureType.rocket;
                            sci.superstructure = HexagonSuperstructure.rocketProtuberanets;
                        }
                        if (shs.superstructure is SteamcopterGyrorotatorScript)
                        {
                            sci.superstructureType = HexagonSuperstructureType.engine;
                            sci.superstructure = HexagonSuperstructure.engineGyrorotator;
                        }
                        if (shs.superstructure is SteamcopterTurbineScript)
                        {
                            sci.superstructureType = HexagonSuperstructureType.engine;
                            sci.superstructure = HexagonSuperstructure.engineTurbine;
                        }
                        if (shs.superstructure is SuperMagnetosphereScript)
                        {
                            sci.superstructureType = HexagonSuperstructureType.superEquipment;
                            sci.superstructure = HexagonSuperstructure.superMagnetosphere;
                        }
                    }

                    for (int k = 0; k < 6; k++)
                    {
                        if (shs.SteamcopterArmorPlateArray[k] != null)
                        {
                            sci.superstructureType = HexagonSuperstructureType.armorPlate;
                            sci.armorPlatePositions.Add(k);
                        }
                    }
                    list.Add(sci);
                }
            }
        return list;
    }

    public static SteamcopterCellInfo GetSteamcopterCellInfo(CellNumber cellNumber)
    {
        List<SteamcopterCellInfo> list = GetSteamcopterConfiguration();
        return list.Find(item => (item.cellNumber.Equals(cellNumber)));
    }

    public static int GetHexagonalCellCost(CellNumber cellNumber)
    {
        CalculateHexagonalCellCost();
        return hexagonalCellCost;
    }

    public static void AddSteamcopterArmorPlate(SteamcopterCellInfo cellInfo, int cost)
    {
        SteamcopterManager.Instance.CreateSteamcopterArmorPlate(new SteamcopterCellInfo(cellInfo.cellNumber, HexagonSuperstructureType.armorPlate, HexagonSuperstructure.none, cellInfo.armorPlatePositions));
        AudioSource.PlayClipAtPoint(BaseOfSounds.Instance.constructionOfSuperstructure, SteamcopterManager.Instance.steamcopterCellArray[cellInfo.cellNumber.x, cellInfo.cellNumber.y].gameObject.transform.position);
        resources -= cost;
    }

    public static void RemoveSteamcopterArmorPlate(SteamcopterCellInfo cellInfo, int cost)
    {
        AudioSource.PlayClipAtPoint(BaseOfSounds.Instance.saleOfCell, SteamcopterManager.Instance.steamcopterCellArray[cellInfo.cellNumber.x, cellInfo.cellNumber.y].gameObject.transform.position);
        resources += cost * cellInfo.armorPlatePositions.Count;
        SteamcopterManager.Instance.RemoveSteamcopterArmorPlate(new SteamcopterCellInfo(cellInfo.cellNumber, HexagonSuperstructureType.armorPlate, HexagonSuperstructure.none, cellInfo.armorPlatePositions));
    }

    public static void UpdateSteamcopterCell(SteamcopterCellInfo cellInfo, int cost)
    {
        SteamcopterCellInfo _cellInfo = GetSteamcopterCellInfo(cellInfo.cellNumber);

        if ((cellInfo.superstructureType == HexagonSuperstructureType.none) &&
            (_cellInfo.superstructureType != HexagonSuperstructureType.none))
        {
            AudioSource.PlayClipAtPoint(BaseOfSounds.Instance.saleOfCell, SteamcopterManager.Instance.steamcopterCellArray[cellInfo.cellNumber.x, cellInfo.cellNumber.y].gameObject.transform.position);
            SteamcopterManager.Instance.RemoveSteamcopterSuperstructure(cellInfo);
        }
        else
        {
            SteamcopterManager.Instance.CreateSteamcopterSuperstructure(cellInfo);
            AudioSource.PlayClipAtPoint(BaseOfSounds.Instance.constructionOfSuperstructure, SteamcopterManager.Instance.steamcopterCellArray[cellInfo.cellNumber.x, cellInfo.cellNumber.y].gameObject.transform.position);
        }
        resources -= cost;
    }

    public static void RemoveHexagonalCell(CellNumber cellNumber)
    {
        AudioSource.PlayClipAtPoint(BaseOfSounds.Instance.saleOfCell, SteamcopterManager.Instance.steamcopterCellArray[cellNumber.x, cellNumber.y].gameObject.transform.position);
        SteamcopterManager.Instance.RemoveSteamcopterHexagon(new SteamcopterCellInfo(cellNumber, HexagonSuperstructureType.none, HexagonSuperstructure.none, null));
        resources += GetHexagonalCellCost(cellNumber) / 2;
    }

    public static void AddHexagonalCell(CellNumber cellNumber)
    {
        resources -= GetHexagonalCellCost(cellNumber);
        SteamcopterManager.Instance.CreateSteamcopterHexagon(new SteamcopterCellInfo(cellNumber, HexagonSuperstructureType.none, HexagonSuperstructure.none, null));
        AudioSource.PlayClipAtPoint(BaseOfSounds.Instance.constructionOfCell, SteamcopterManager.Instance.steamcopterCellArray[cellNumber.x, cellNumber.y].gameObject.transform.position);
    }
}

public class SteamcopterCellInfo
{
    public CellNumber cellNumber;
    public HexagonSuperstructureType superstructureType;
    public HexagonSuperstructure superstructure;
    public List<int> armorPlatePositions;

    public SteamcopterCellInfo(CellNumber CellNumber,  HexagonSuperstructureType SuperstructureType, HexagonSuperstructure Superstructure, List<int> ArmorPlatePositions)
    {
        cellNumber = CellNumber;
        superstructureType = SuperstructureType;
        superstructure = Superstructure;
        armorPlatePositions = ArmorPlatePositions;
    }
}

public enum HexagonSuperstructureType
{
    none,
    commandCell,
    superEquipment,
    armorPlate,
    autoTurret,
    gun,
    rocket,
    engine,
}

public enum HexagonSuperstructure
{
    //NULL,
    none,
    commandCell,
    superMagnetosphere,
    autoNeedle,
    autoPointDefense,
    autoJaws,
    autoSuppressor,
    gunShestoper,
    gunRhino,
    rocketProtuberanets,
    engineTurbine,
    engineGyrorotator,
}