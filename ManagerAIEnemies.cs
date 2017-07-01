using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManagerAIEnemies : UnitySingleton<ManagerAIEnemies>
{
    public GameObject boss1Prefab;
    public GameObject enemy1Prefab;
    public GameObject enemy2Prefab;
    public GameObject enemy3Prefab;
    public EndLevelScript endLevelScript;

    private int numberOfWaves = 1;
    public Vector3 deltaSteamcopterCoordinate;
    private float timeOfBossDeath = -1;
    private float delayAfterBossDeath = 2;

    public List<GameObject> objectVerticesOfGraph;
    public List<Vector3> verticesOfGraph = new List<Vector3>();
    public List<float> distance = new List<float>();
    public bool[,] GR;
    public List<EnemyAIBasic> enemyAIBasicList = new List<EnemyAIBasic>();

    public void AddEnemyAIBasic(EnemyAIBasic enemyAIBasic)
    {
        enemyAIBasicList.Add(enemyAIBasic);
        verticesOfGraph.Add(enemyAIBasic.gameObject.transform.position);
    }

    public void RemoveEnemyAIBasic(EnemyAIBasic enemyAIBasic)
    {
        verticesOfGraph.RemoveAt(enemyAIBasicList.IndexOf(enemyAIBasic) + (objectVerticesOfGraph.Count) + 1);
        enemyAIBasicList.Remove(enemyAIBasic);
    }

    public void BossDestroyed()
    {
        timeOfBossDeath = Time.time;
        foreach (EnemyAIBasic eaib in enemyAIBasicList)
        {
            eaib.Sleep(10000);
        }
    }

    public void UpdateSteamcopterCoordinate()
    {
        List<SteamcopterCellInfo> list = PlayerInfoManager.GetSteamcopterConfiguration();
        Vector3 vec = new Vector3(0, 0, 0);
        int n = 0;
        foreach (SteamcopterCellInfo sci in list)
        {
            if (sci.superstructureType == HexagonSuperstructureType.commandCell)
            {
                vec += SteamcopterManager.Instance.steamcopterCellArray[sci.cellNumber.x, sci.cellNumber.y].gameObject.transform.localPosition;
                n++;
            }
        }
        vec /= n;
        deltaSteamcopterCoordinate = vec;
    }

    private List<int> FindAdjacencyList(Vector3 point)
    {
        List<int> list = new List<int>();
        for (int j = 0; j < verticesOfGraph.Count; j++)
        {
            bool flag = true;
            RaycastHit[] RHList = Physics.RaycastAll(point, verticesOfGraph[j] - point);
            foreach (RaycastHit rh in RHList)
            {
                if (rh.collider.gameObject.tag == "Environment")
                {
                    flag = false;
                    break;
                }
            }

            if (flag)
            {
                list.Add(j);
            }
        }
        return list;
    }

    private void FindPath()
    {
        GR = new bool[verticesOfGraph.Count, verticesOfGraph.Count];
        for (int i = 0; i < verticesOfGraph.Count; i++)
            for (int j = 0; j < verticesOfGraph.Count; j++)
            {
                GR[i, j] = false;
            }
        verticesOfGraph[0] = GetSteamcopterCoordinate();
        for (int i = 0; i < enemyAIBasicList.Count; i++)
        {
            verticesOfGraph[1 + objectVerticesOfGraph.Count + i] = enemyAIBasicList[i].gameObject.transform.position;
        }

        for (int i = 0; i < verticesOfGraph.Count; i++)
            for (int j = 0; j < verticesOfGraph.Count; j++)
            {
                bool flag = true;
                RaycastHit[] RHList = Physics.RaycastAll(verticesOfGraph[i], verticesOfGraph[j] - verticesOfGraph[i], (verticesOfGraph[j] - verticesOfGraph[i]).magnitude);
                foreach (RaycastHit rh in RHList)
                {
                    if (rh.collider.gameObject.tag == "Environment")
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    GR[i, j] = true;
                    GR[j, i] = true;
                }
            }
        {
            int index = 0;
            int u;
            int m = 1;
            distance = new List<float>();
            List<bool> visited = new List<bool>();
            for (int i = 0; i < verticesOfGraph.Count; i++)
            {
                distance.Add(10000); visited.Add(false);
            }
            distance[0] = 0;
            for (int count = 0; count < verticesOfGraph.Count - 1; count++)
            {
                float min = 10000;
                for (int i = 0; i < verticesOfGraph.Count; i++)
                    if (!visited[i] && distance[i] <= min)
                    {
                        min = distance[i]; index = i;
                    }
                u = index;
                visited[u] = true;
                for (int i = 0; i < verticesOfGraph.Count; i++)
                    if (!visited[i] && GR[u, i] && distance[u] != 10000 &&
                    (distance[u] + (verticesOfGraph[u] - verticesOfGraph[i]).magnitude) < distance[i])
                        distance[i] = distance[u] + (verticesOfGraph[u] - verticesOfGraph[i]).magnitude;
            }
        }
    }

    private void InitGraph()
    {
        verticesOfGraph.Add(GetSteamcopterCoordinate());
        foreach (GameObject go in objectVerticesOfGraph)
        {
            verticesOfGraph.Add(go.transform.position);
        }
    }

    public float GetDistanceToTarget(EnemyAIBasic eAIB)
    {
        if (distance.Count == 0)
            return 10000;

        float distanceToTarget = -1;
        int index = 1 + objectVerticesOfGraph.Count + enemyAIBasicList.IndexOf(eAIB);
        distanceToTarget = distance[index];
        return distanceToTarget;
    }

    public Vector3 GetDestinationPoint(EnemyAIBasic eAIB)
    {
        Vector3 destinationPoint = SteamcopterManager.Instance.gameObject.transform.position;
        int index = 1 + objectVerticesOfGraph.Count + enemyAIBasicList.IndexOf(eAIB);
        for (int i = 0; i < verticesOfGraph.Count; i++)
        {
            if (GR!=null && GR[index, i] &&
                (Mathf.Abs(distance[i] + (verticesOfGraph[index] - verticesOfGraph[i]).magnitude - distance[index]) < 0.1f))
            {
                destinationPoint = verticesOfGraph[i];
                break;
            }
        }
        return destinationPoint;
    }

    public Vector3 GetSteamcopterCoordinate()
    {
        return deltaSteamcopterCoordinate + SteamcopterManager.Instance.gameObject.transform.position;
    }

    protected override void Awake()
    {
        base.Awake();
        InitGraph();
    }

    void FixedUpdate()
    {
        FindPath();
        if ((!LevelLoader.LevelTrainingScriptCompleted) || (!LevelLoader.LevelLoadScriptCompleted) || (!LevelLoader.loadScriptPreparationDone))
            return;
    }

	void Update () 
    {
        if ((timeOfBossDeath != -1) && ((timeOfBossDeath + delayAfterBossDeath) < Time.time))
        {
            timeOfBossDeath = -1;
            endLevelScript.StartScript();
        }
        if ((numberOfWaves == 2) && (enemyAIBasicList.Count == 0) && (timeOfBossDeath == -1))
        {
            timeOfBossDeath = Time.time;
        }
        if ((numberOfWaves == 2) && (enemyAIBasicList.Count == 0) && ((timeOfBossDeath + delayAfterBossDeath) < Time.time))
        {
            timeOfBossDeath = -1;
            InputManager.Instance.ActivateFinalResultInterface(true);
        }
	}
}
