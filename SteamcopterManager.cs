using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SteamcopterManager : UnitySingleton<SteamcopterManager>
{
    public float linearPowerOfEngine;
    public float rotationalPowerOfEngine;
    public const int steamcopterCellArrayAmount = 20;

    public SteamcopterHexagonScript[,] steamcopterCellArray = new SteamcopterHexagonScript[steamcopterCellArrayAmount, steamcopterCellArrayAmount];
    public Vector3 cellPositionOffset;
    public Rigidbody steamcopterRigidbody;
    public Vector3 lastMoveDirectory = Vector3.zero;

    private bool isDeath = false;
    private float timeOfDeath = -1;
    private float delayAfterDeath = 5;
    public float maxSpeed = 9;
    public float speed = 0;

    public SuperEquipmentAbstract superEquipment = null;
    public List<Collider> collidersList = new List<Collider>();
    private List<BigGunAbstract> steamcopterBigGunList = new List<BigGunAbstract>();
    private List<AutoTurretAbstract> steamcopterAutoTurretList = new List<AutoTurretAbstract>();
    private List<RocketAbstract> steamcopterRocketLauncherList = new List<RocketAbstract>();

    protected override void Awake()
    {
        base.Awake();
        PlayerInfoManager.LoadPlayerInfo();
        steamcopterRigidbody = this.gameObject.GetComponent<Rigidbody>();
    }

	void Start () 
    {
        BuildSteamcopter(PlayerInfoManager.GetStartSteamcopterConfiguration());
        ManagerAIEnemies.Instance.UpdateSteamcopterCoordinate();
	}
	
	void Update () 
    {
        speed = steamcopterRigidbody.velocity.magnitude;
        if (steamcopterRigidbody.IsSleeping())
        {
            steamcopterRigidbody.WakeUp();
        }

        if ((timeOfDeath != -1) && ((timeOfDeath + delayAfterDeath) < Time.time))
        {
            timeOfDeath = -1;
            InputManager.Instance.ActivateFinalResultInterface(false);
        }
	}

    void FixedUpdate()
    {
        if (steamcopterRigidbody.velocity.magnitude > maxSpeed)
        {
            steamcopterRigidbody.velocity = this.gameObject.GetComponent<Rigidbody>().velocity.normalized * maxSpeed;
        }

        {
            List<AutoTurretAbstract> readyPointDefenseList = new List<AutoTurretAbstract>();
            List<Collider> enemyRocketList = new List<Collider>();

            foreach (AutoTurretAbstract sg in steamcopterAutoTurretList)
            {
                if ((sg is AutoPointDefenseScript))
                {
                    if (sg.GetTimeLeftToRecharge() == 0)
                    {
                        readyPointDefenseList.Add(sg);
                    }
                }
            }

            Collider[] colliders = Physics.OverlapSphere(this.gameObject.transform.position, 20);
            for (int i = 0; i < colliders.Length; i++)
            {
                if ((colliders[i].gameObject.transform.position.y > -5) &&
                    (colliders[i].gameObject.tag == "EnemyRocket") &&
                    (!colliders[i].gameObject.GetComponent<RocketScript>().underTheSight))
                {
                    enemyRocketList.Add(colliders[i]);
                }
            }

            foreach (AutoTurretAbstract ata in readyPointDefenseList)
            {
                if (enemyRocketList.Count == 0)
                {
                    continue;
                }
                enemyRocketList[0].gameObject.GetComponent<RocketScript>().underTheSight = true;
                ata.OpenFire(enemyRocketList[0].gameObject);
                enemyRocketList.RemoveAt(0);
            }
        }

        //------------------------------------------------------------------------------------------------

        {
            List<AutoTurretAbstract> readyJawsList = new List<AutoTurretAbstract>();
            foreach (AutoTurretAbstract ata in steamcopterAutoTurretList)
            {
                if ((ata is AutoJawsScript))
                {
                    if (ata.GetTimeLeftToRecharge() == 0)
                    {
                        readyJawsList.Add(ata);
                    }
                }
            }

            foreach (AutoTurretAbstract ata in readyJawsList)
            {
                Collider[] _colliders = Physics.OverlapSphere(ata.gameObject.transform.position, 3);
                for (int i = 0; i < _colliders.Length; i++)
                {
                    if (((_colliders[i].gameObject.tag == "Enemy") || (_colliders[i].gameObject.tag == "EnemyBasis")) 
                        && (_colliders[i].gameObject.GetComponent<AbstractStateComponent>() != null))
                    {
                        ata.OpenFire(_colliders[i].gameObject);
                        break;
                    }
                }
            }
        }

        //---------------------------------------------------------------------------------------

        {
            List<AutoTurretAbstract> readyAutoTurretList = new List<AutoTurretAbstract>();
            foreach (AutoTurretAbstract ata in steamcopterAutoTurretList)
            {
                if ((!(ata is AutoPointDefenseScript)) && (!(ata is AutoJawsScript)))
                {
                    if (ata.GetTimeLeftToRecharge() == 0)
                    {
                        readyAutoTurretList.Add(ata);
                    }
                }
            }

            Collider[] colliders = Physics.OverlapSphere(this.gameObject.transform.position, 15);
            List<Collider> colliderList = new List<Collider>();
            colliderList.AddRange(colliders);

            for (int i = colliderList.Count-1 ; i >= 0; i--)
                {
                    if (!((colliderList[i].gameObject.tag == "Enemy") || (colliderList[i].gameObject.tag == "EnemyBasis")) 
                        || (colliderList[i].gameObject.GetComponent<AbstractStateComponent>() == null))
                {
                    colliderList.RemoveAt(i);
                }
            }

            if (colliderList.Count > 0)
            {
                foreach (AutoTurretAbstract ata in readyAutoTurretList)
                {
                    Collider col = colliderList[Random.Range(0, colliderList.Count - 1)];

                    ata.OpenFire(col.gameObject);

                }
            }
        }
    }

    public void DestroyBigGun(BigGunAbstract bga)
    {
        steamcopterBigGunList.Remove(bga);
    }

    public void DestroyAutoTurret(AutoTurretAbstract ata)
    {
        steamcopterAutoTurretList.Remove(ata);
    }

    public void DestroyRocketLauncher(RocketAbstract ra)
    {
        steamcopterRocketLauncherList.Remove(ra);
    }

    public void DestroySuperEquipment(SuperEquipmentAbstract sea)
    {
        if (superEquipment == sea)
        {
            superEquipment = sea;
        }
    }

    public void ShootBigGun(Vector3 Direction)
    {
        BigGunAbstract bigGunForShot = null;

        foreach (BigGunAbstract bg in steamcopterBigGunList)
        {
            if (bg.GetTimeLeftToRecharge() == 0)
            {
                bigGunForShot = bg;
                continue;
            }
        }
        if (bigGunForShot != null)
        {
            bigGunForShot.OpenFire(Direction);
        }
    }

    public void ShootRocket(Vector3 Direction)
    {
        RocketAbstract rocketForShot = null;

        foreach (RocketAbstract ra in steamcopterRocketLauncherList)
        {
            if (ra.GetTimeLeftToRecharge() == 0)
            {
                rocketForShot = ra;
                continue;
            }
        }
        if (rocketForShot != null)
        {
            rocketForShot.OpenFire(Direction);
        }
    }

    public void UseSuperEquipment()
    {
        if (superEquipment != null)
        {
            superEquipment.Perform();
        }
    }

    public void DestroySteamcopterCommandCell()
    {
        if (!isDeath)
        {
            for (int i = 0; i < steamcopterCellArrayAmount; i++)
                for (int j = 0; j < steamcopterCellArrayAmount; j++)
                {
                    if (steamcopterCellArray[i, j] != null)
                    {
                        if (!(steamcopterCellArray[i, j] is SteamcopterCommandCellScript))
                        {
                            steamcopterCellArray[i, j].Destroy();
                        }
                    }
                }
            steamcopterRigidbody.isKinematic = true;
            timeOfDeath = Time.time;
            isDeath = true;

            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < steamcopterCellArrayAmount; i++)
                for (int j = 0; j < steamcopterCellArrayAmount; j++)
                {
                    if (steamcopterCellArray[i, j] != null)
                    {
                        if ((steamcopterCellArray[i, j] is SteamcopterCommandCellScript))
                        {
                            list.Add(steamcopterCellArray[i, j].gameObject);
                        }
                    }
                }
            EventOnFieldManager.Instance.VisualizeDestructionOfCommandCell(list);
        }
    }

    public void BuildSteamcopter(List<SteamcopterCellInfo> steamcopterCellInfoList)
    {
        CalculatePositionOffset(steamcopterCellInfoList);

        foreach (SteamcopterCellInfo sci in steamcopterCellInfoList)
        {
            CreateSteamcopterHexagon(sci);
            CreateSteamcopterSuperstructure(sci);
            CreateSteamcopterArmorPlate(sci);
        }
        CreateCommandCellSuperstructure();
    }

    private void CreateCommandCellSuperstructure()
    {
        Vector3 position = new Vector3(0, 0, 0);
        int n = 0;
        for (int i = 0; i < steamcopterCellArrayAmount; i++)
            for (int j = 0; j < steamcopterCellArrayAmount; j++)
            {
                if (steamcopterCellArray[i, j] != null)
                {
                    if ((steamcopterCellArray[i, j] is SteamcopterCommandCellScript))
                    {
                        n++;
                        position += steamcopterCellArray[i, j].gameObject.transform.position;
                    }
                }
            }
        position /= n;
        position += new Vector3(0, -0.55f, 0);

        GameObject prefab = BaseOfPrefabs.Instance.commandCell;
        GameObject commandCellSuperstructure = (GameObject)Instantiate(prefab, position, prefab.transform.rotation);
        commandCellSuperstructure.transform.parent = this.gameObject.transform;
    }

    public void CalculatePositionOffset(List<SteamcopterCellInfo> steamcopterCellInfoList)
    {
        CellNumber minCellNumber = new CellNumber(steamcopterCellArrayAmount, steamcopterCellArrayAmount);
        CellNumber maxCellNumber = new CellNumber(0, 0);
        foreach (SteamcopterCellInfo sci in steamcopterCellInfoList)
        {
            if (minCellNumber.x > sci.cellNumber.x)
            {
                minCellNumber.x = sci.cellNumber.x;
            }
            if (minCellNumber.y > sci.cellNumber.y)
            {
                minCellNumber.y = sci.cellNumber.y;
            }

            if (maxCellNumber.x < sci.cellNumber.x)
            {
                maxCellNumber.x = sci.cellNumber.x;
            }
            if (maxCellNumber.y < sci.cellNumber.y)
            {
                maxCellNumber.y = sci.cellNumber.y;
            }
        }

        Vector3 positionOffset =
            CellNumber.GetCellPosition(new CellNumber(((minCellNumber.x + maxCellNumber.x) / 2) + ((minCellNumber.x + maxCellNumber.x) % 2),
                ((minCellNumber.y + maxCellNumber.y) / 2) + ((minCellNumber.y + maxCellNumber.y) % 2)));
        cellPositionOffset = positionOffset;
    }

    public void CreateSteamcopterHexagon(SteamcopterCellInfo steamcopterCellInfo)
    {
        if (steamcopterCellInfo.superstructureType != HexagonSuperstructureType.commandCell)
        {
            SteamcopterHexagonScript steamcopterHexagonScript;
            float Angle = (SteamcopterManager.Instance.gameObject.transform.rotation.eulerAngles.y / 180) * Mathf.PI;
            Vector3 position = CellNumber.RotateVector(CellNumber.GetCellPosition(steamcopterCellInfo.cellNumber) - SteamcopterManager.Instance.cellPositionOffset, Angle)
                + SteamcopterManager.Instance.gameObject.transform.position;
            GameObject prefab = BaseOfPrefabs.Instance.hexagon;
            GameObject hexagon = (GameObject)Instantiate(prefab, position, prefab.transform.rotation);
            steamcopterHexagonScript = hexagon.GetComponent<SteamcopterHexagonScript>();
            steamcopterHexagonScript.steamcopterHexagonNumber = steamcopterCellInfo.cellNumber;
            steamcopterCellArray[steamcopterCellInfo.cellNumber.x, steamcopterCellInfo.cellNumber.y] = steamcopterHexagonScript;
            hexagon.transform.Rotate(new Vector3(0,0,1), this.gameObject.transform.rotation.eulerAngles.y);
            hexagon.transform.parent = this.gameObject.transform;

            collidersList.Add(hexagon.GetComponent<Collider>());
        }
        else
        {
            SteamcopterCommandCellScript steamcopterCommandCellScript;
            float Angle = (SteamcopterManager.Instance.gameObject.transform.rotation.eulerAngles.y / 180) * Mathf.PI;
            Vector3 position = CellNumber.RotateVector(CellNumber.GetCellPosition(steamcopterCellInfo.cellNumber) - SteamcopterManager.Instance.cellPositionOffset, Angle)
                + SteamcopterManager.Instance.gameObject.transform.position;
            GameObject prefab = BaseOfPrefabs.Instance.hexagon;
            GameObject hexagon = (GameObject)Instantiate(prefab, position, prefab.transform.rotation);
            Destroy(hexagon.GetComponent<SteamcopterHexagonScript>());
            hexagon.AddComponent<SteamcopterCommandCellScript>();
            steamcopterCommandCellScript = hexagon.GetComponent<SteamcopterCommandCellScript>();
            steamcopterCommandCellScript.steamcopterHexagonNumber = steamcopterCellInfo.cellNumber;
            steamcopterCellArray[steamcopterCellInfo.cellNumber.x, steamcopterCellInfo.cellNumber.y] = steamcopterCommandCellScript;
            hexagon.transform.parent = this.gameObject.transform;

            collidersList.Add(hexagon.GetComponent<Collider>());
            collidersList.Add(hexagon.GetComponent<Collider>());
        }
    }

    public void CreateSteamcopterSuperstructure(SteamcopterCellInfo steamcopterCellInfo)
    {
        GameObject prefab = BaseOfPrefabs.Instance.GetSteamcopterSuperstructurePrefab(steamcopterCellInfo);
        if (prefab != null)
        {
            SteamcopterHexagonScript steamcopterHexagonScript = steamcopterCellArray[steamcopterCellInfo.cellNumber.x, steamcopterCellInfo.cellNumber.y];
            float Angle = SteamcopterManager.Instance.gameObject.transform.rotation.eulerAngles.y;
            GameObject superstructure = (GameObject)Instantiate(prefab, new Vector3(0, 0, 0), prefab.transform.rotation);
            Vector3 rotationVector = new Vector3(0, 1, 0);
            superstructure.transform.Rotate(rotationVector , Angle);
            superstructure.transform.parent = steamcopterHexagonScript.gameObject.transform;
            
            switch (steamcopterCellInfo.superstructureType)
            {
                case HexagonSuperstructureType.autoTurret:
                    superstructure.GetComponent<AutoTurretAbstract>().sectionOfShip = steamcopterHexagonScript;
                    steamcopterHexagonScript.superstructure = superstructure.GetComponent<AutoTurretAbstract>();
                    steamcopterAutoTurretList.Add(superstructure.GetComponent<AutoTurretAbstract>());
                    break;

                case HexagonSuperstructureType.gun:
                    superstructure.GetComponent<BigGunAbstract>().sectionOfShip = steamcopterHexagonScript;
                    steamcopterHexagonScript.superstructure = superstructure.GetComponent<BigGunAbstract>();
                    steamcopterBigGunList.Add(superstructure.GetComponent<BigGunAbstract>());
                    break;

                case HexagonSuperstructureType.rocket:
                    superstructure.GetComponent<RocketAbstract>().sectionOfShip = steamcopterHexagonScript;
                    steamcopterHexagonScript.superstructure = superstructure.GetComponent<RocketAbstract>();
                    steamcopterRocketLauncherList.Add(superstructure.GetComponent<RocketAbstract>());
                    break;

                case HexagonSuperstructureType.superEquipment:
                    superstructure.GetComponent<SuperEquipmentAbstract>().sectionOfShip = steamcopterHexagonScript;
                    steamcopterHexagonScript.superstructure = superstructure.GetComponent<SuperEquipmentAbstract>();
                    superEquipment = superstructure.GetComponent<SuperEquipmentAbstract>();
                    break;
            }

            switch (steamcopterCellInfo.superstructure)
            {
                case HexagonSuperstructure.engineGyrorotator:
                    superstructure.GetComponent<SteamcopterGyrorotatorScript>().steamcopterHexagon = steamcopterHexagonScript;
                    steamcopterHexagonScript.superstructure = superstructure.GetComponent<SteamcopterGyrorotatorScript>();
                    break;

                case HexagonSuperstructure.engineTurbine:
                    superstructure.GetComponent<SteamcopterTurbineScript>().steamcopterHexagon = steamcopterHexagonScript;
                    steamcopterHexagonScript.superstructure = superstructure.GetComponent<SteamcopterTurbineScript>();
                    break;
            }

            superstructure.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public void CreateSteamcopterArmorPlate(SteamcopterCellInfo steamcopterCellInfo)
    {
        if (steamcopterCellInfo.armorPlatePositions != null)
        {
            SteamcopterHexagonScript steamcopterHexagonScript = steamcopterCellArray[steamcopterCellInfo.cellNumber.x, steamcopterCellInfo.cellNumber.y];
            GameObject prefab = BaseOfPrefabs.Instance.armorPlate;
            foreach (int pos in steamcopterCellInfo.armorPlatePositions)
            {
                GameObject armorPlate = (GameObject)Instantiate(prefab, new Vector3(0, 0, 0), prefab.transform.rotation);
                armorPlate.GetComponent<SteamcopterArmorPlateScript>().steamcopterHexagon = steamcopterHexagonScript;
                steamcopterHexagonScript.SteamcopterArmorPlateArray[pos] = armorPlate.GetComponent<SteamcopterArmorPlateScript>();
                armorPlate.transform.Rotate(new Vector3(0, 0, pos * 60));
                armorPlate.transform.Rotate(new Vector3(0, 0, SteamcopterManager.Instance.gameObject.transform.rotation.eulerAngles.y));
                armorPlate.transform.parent = steamcopterHexagonScript.gameObject.transform;
                armorPlate.transform.localPosition = new Vector3(0, 0, 0);

                collidersList.Add(armorPlate.GetComponent<Collider>());
            }
        }
    }

    public void RemoveSteamcopterArmorPlate(SteamcopterCellInfo steamcopterCellInfo)
    {
        if (steamcopterCellArray[steamcopterCellInfo.cellNumber.x, steamcopterCellInfo.cellNumber.y] != null)
        {
            for (int i = 0; i < 6; i++)
            {
                if (steamcopterCellArray[steamcopterCellInfo.cellNumber.x, steamcopterCellInfo.cellNumber.y].SteamcopterArmorPlateArray[i] != null)
                {
                    steamcopterCellArray[steamcopterCellInfo.cellNumber.x, steamcopterCellInfo.cellNumber.y].SteamcopterArmorPlateArray[i].Destroy();
                }
            }
        }
        else
        {
            Debug.LogError("SteamcopterManager::RemoveSteamcopterArmorPlate:  Null reference");
        }
    }

    public void RemoveSteamcopterSuperstructure(SteamcopterCellInfo steamcopterCellInfo)
    {
        if (steamcopterCellArray[steamcopterCellInfo.cellNumber.x, steamcopterCellInfo.cellNumber.y] != null)
        {
            if (steamcopterCellArray[steamcopterCellInfo.cellNumber.x, steamcopterCellInfo.cellNumber.y].superstructure != null)
            {
                steamcopterCellArray[steamcopterCellInfo.cellNumber.x, steamcopterCellInfo.cellNumber.y].superstructure.Destroy();
            }
        }
        else
        {
            Debug.LogError("SteamcopterManager::RemoveSteamcopterSuperstructure:  Null reference");
        }
    }

    public void RemoveSteamcopterHexagon(SteamcopterCellInfo steamcopterCellInfo)
    {
        if (steamcopterCellArray[steamcopterCellInfo.cellNumber.x, steamcopterCellInfo.cellNumber.y] != null)
        {
            steamcopterCellArray[steamcopterCellInfo.cellNumber.x, steamcopterCellInfo.cellNumber.y].Sell();
            steamcopterCellArray[steamcopterCellInfo.cellNumber.x, steamcopterCellInfo.cellNumber.y] = null;
        }
        else
        {
            Debug.LogError("SteamcopterManager::RemoveSteamcopterHexagon:  Null reference");
        }
    }

    public List<CellNumber> GetFallsOffCells(CellNumber cellNumber)
    {
        List<CellNumber> list = new List<CellNumber>();
        int[,] array = new int[steamcopterCellArrayAmount, steamcopterCellArrayAmount];
        for (int i = 0; i < steamcopterCellArrayAmount; i++)
            for (int j = 0; j < steamcopterCellArrayAmount; j++)
            {
                if (steamcopterCellArray[i, j] == null)
                {
                    array[i, j] = -1;
                }
                else
                {
                    array[i, j] = 1;
                }
            }
        array[cellNumber.x, cellNumber.y] = -1;

        List<CellNumber> cellNumberPool = new List<CellNumber>();
        for (int i = 0; i < steamcopterCellArrayAmount; i++)
            for (int j = 0; j < steamcopterCellArrayAmount; j++)
            {
                if ((steamcopterCellArray[i, j] != null) && (steamcopterCellArray[i, j] is SteamcopterCommandCellScript))
                {
                    cellNumberPool.Add(new CellNumber(i, j));
                }
            }

        while (cellNumberPool.Count > 0)
        {
            if (array[cellNumberPool[0].x, cellNumberPool[0].y] == 1)
            {
                array[cellNumberPool[0].x, cellNumberPool[0].y] = 0;
                cellNumberPool.AddRange(cellNumberPool[0].GetNeighborCells());
            }

            cellNumberPool.RemoveAt(0);
        }

        for (int i = 0; i < steamcopterCellArrayAmount; i++)
            for (int j = 0; j < steamcopterCellArrayAmount; j++)
            {
                if (array[i, j] == 1)
                {
                    list.Add(new CellNumber(i, j));
                }
            }
        return list;
    }

    public void SetMoveDirectory(Vector3 moveDirectory)
    {
        float angle = Vector3.Angle(moveDirectory, Vector3.forward);

        if (Vector3.Angle(moveDirectory, Vector3.left) < 90f)
        {
            angle = 360 - angle;
        }

        float angleRotation = +angle - this.gameObject.transform.rotation.eulerAngles.y;
        if (angleRotation < -180)
        {
            angleRotation += 360;
        }

        if (angleRotation > 180)
        {
            angleRotation -= 360;
        }

        steamcopterRigidbody.AddTorque(Mathf.Sign(angleRotation)
            * Vector3.up * Mathf.Abs(angleRotation) / 180 * Time.deltaTime * rotationalPowerOfEngine, ForceMode.Impulse);

        float factor = 1 - Mathf.Abs(angleRotation) / 180;
        steamcopterRigidbody.AddRelativeForce(Vector3.forward * factor * Time.deltaTime * linearPowerOfEngine, ForceMode.Impulse);
        lastMoveDirectory = moveDirectory * factor;
    }
}
