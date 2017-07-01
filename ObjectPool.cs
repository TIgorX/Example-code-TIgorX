using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : UnitySingleton<ObjectPool>
{
    public int startProjectilesPool = 5;
    public int maxProjectilesPool = 10;

    public GameObject projectile;
    public List<GameObject> projectilesList = new List<GameObject>();
    public Vector3 poolPosition = new Vector3(500, 500, 0);

    protected override void Awake()
    {
        base.Awake();
        CreatePool(projectile, ref projectilesList, startProjectilesPool);
    }

    private void CreatePool(GameObject gameObject, ref List<GameObject> list, int number)
    {
        for (int i = 0; i < number; i++)
        {
            GameObject go = (GameObject)Instantiate(gameObject, poolPosition, Quaternion.identity);
            go.SetActive(false);
            list.Add(go);
        }
    }

    public GameObject TakeWithPool(GameObject prefab)
    {
        GameObject go;
        List<GameObject> list = new List<GameObject>();

        if (prefab == projectile)
        {
            list = projectilesList;
        }
        if (list.Count == 0)
        {
            CreatePool(prefab, ref list, 1);
        }
        go = list[0];
        list.RemoveAt(0);
        go.SetActive(true);

        return go;
    }

    public void ReturnToPool(GameObject gameObject)
    {
        List<GameObject> list = new List<GameObject>();
        int maxPool = -1;

        if (gameObject.name == (projectile.name + "(Clone)"))
        {
            list = projectilesList;
            maxPool = maxProjectilesPool;
        }
        gameObject.SetActive(false);
        gameObject.transform.position = poolPosition;
        list.Add(gameObject);
        if (list.Count > maxPool)
        {
            GameObject go = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            Destroy(go);
        }
    }
}
