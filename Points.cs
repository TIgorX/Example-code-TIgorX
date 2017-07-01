using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
[ExecuteInEditMode]
#endif

public class Points : MonoBehaviour {
    public bool loop = true;
    public bool smoothTurn = true;
    public float radius = 10.0f;
    public float speed = 50.0f;
    public float respawnTime = 3.0f;

    [HideInInspector] public Vector3[] cords;

    [SerializeField]  private Transform[] _points = new Transform[0];
    [SerializeField] private GameObject[] _objects = new GameObject[0];

	void Awake () {
        int i = _points != null ? _points.Length : 0;
        cords = new Vector3[i];
        while (--i > -1)
        {
            cords[i] = _points[i].position;
        }

        foreach (GameObject go in _objects)
        {
            var om = go.GetComponent<ObjectMover>();
            if(om == null)
                om = go.AddComponent<ObjectMover>();
            om.points = this;
        }
	}

    public void RespawnObject(GameObject go)
    {
        StartCoroutine(coRespawnObject(go));
    }

    private IEnumerator coRespawnObject(GameObject go)
    {
        go.SetActive(false);
        yield return new WaitForSeconds(1);
        go.SetActive(true);
        go.transform.position = cords[0];
    }

#if UNITY_EDITOR
    [SerializeField]
    private Color32 _gizmosColor;

    void OnDrawGizmos()
    {
        Gizmos.color = _gizmosColor;
        int i = _points !=  null? _points.Length : 0;
        while (--i > -1)
        {
            Gizmos.DrawSphere(_points[i].position, 5);
            Gizmos.DrawWireSphere(_points[i].position, radius);
            Handles.Label(_points[i].position, "Point " + i);
            if (i < _points.Length - 1)
                Gizmos.DrawLine(_points[i].position, _points[i + 1].position);
            else
            {
                if (loop)
                    Gizmos.DrawLine(_points[_points.Length - 1].position, _points[0].position);
            }
        }
    }
#endif
}
