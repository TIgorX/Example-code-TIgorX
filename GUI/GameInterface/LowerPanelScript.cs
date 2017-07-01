using UnityEngine;
using System.Collections;

public class LowerPanelScript : MonoBehaviour
{
    private Vector3 positionOffset;

    void Awake()
    {
        positionOffset = this.gameObject.transform.position;
        positionOffset.y = -2;
    }

    void OnEnable()
    {
        this.gameObject.transform.position = positionOffset + GameCameraScript.Instance.gameObject.transform.position;
    }
}
