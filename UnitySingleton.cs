using System.Collections;
using UnityEngine;


public class UnitySingleton<T> : MonoBehaviour
    where T : UnitySingleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            //if (_instance == null)
            //{
            //    CreateIfNotExists();
            //}
            return _instance;
        }
    }

    public static void CreateIfNotExists()
    {
        if (_instance != null) return;

        _instance = FindObjectOfType<T>();

        if (_instance != null) return;

        var go = new GameObject(typeof(T).Name);
        _instance = go.AddComponent<T>();
    }
    public static bool Exists { get { return _instance != null; } }

    protected virtual void Awake()
    {
        _instance = this as T;
        //DontDestroyOnLoad(gameObject);
    }

    //protected virtual void OnDestroy()
    //{
    //    if (_instance == this as T)
    //    {
    //        _instance = null;
    //    }
    //}
}
