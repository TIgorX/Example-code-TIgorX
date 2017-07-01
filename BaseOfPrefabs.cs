using UnityEngine;
using System.Collections;

public class BaseOfPrefabs : UnitySingleton<BaseOfPrefabs>
{
    public GameObject rocket;
    public GameObject turretProjectile;
    public GameObject commandCell;
    public GameObject hexagon;

    public GameObject superMagnetosphere;
    public GameObject autoNeedle;
    public GameObject autoPointDefense;
    public GameObject autoJaws;
    public GameObject autoSuppressor;
    public GameObject gunShestoper;
    public GameObject gunRhino;
    public GameObject rocketProtuberanets;
    public GameObject engineTurbine;
    public GameObject engineGyrorotator;
    public GameObject armorPlate;

    protected override void Awake()
    {
        base.Awake();
    }

    public GameObject GetSteamcopterSuperstructurePrefab(SteamcopterCellInfo steamcopterCellInfo)
    {
        GameObject prefab = null;

        switch (steamcopterCellInfo.superstructure)
        {
            case HexagonSuperstructure.autoNeedle:
                prefab = autoNeedle;
                break;
            case HexagonSuperstructure.gunShestoper:
                prefab = gunShestoper;
                break;
            case HexagonSuperstructure.rocketProtuberanets:
                prefab = rocketProtuberanets;
                break;
            case HexagonSuperstructure.autoPointDefense:
                prefab = autoPointDefense;
                break;
            case HexagonSuperstructure.autoJaws:
                prefab = autoJaws;
                break;
            case HexagonSuperstructure.engineTurbine:
                prefab = engineTurbine;
                break;
            case HexagonSuperstructure.engineGyrorotator:
                prefab = engineGyrorotator;
                break;
            case HexagonSuperstructure.superMagnetosphere:
                prefab = superMagnetosphere;
                break;
            case HexagonSuperstructure.autoSuppressor:
                prefab = autoSuppressor;
                break;
            case HexagonSuperstructure.gunRhino:
                prefab = gunRhino;
                break;
        }

        return prefab;
    }
}
