using UnityEngine;
using System.Collections;

public class LevelLoadScript : MonoBehaviour 
{
    public Camera mainCamera;
    public GameObject mapClouds;
    public GameObject mapLowClouds;
    public GameObject levelLoadCamera;
    public GameObject[] inactiveObjects;

    public int scriptStage = 0;

    void Awake()
    {
        LevelLoader.LevelLoadScriptCompleted = false;
        foreach (GameObject go in inactiveObjects)
        {
            go.SetActive(false);
        }
    }

    public void SetLevelPresetting(LevelLoadInfo levelLoadInfo)
    {
        levelLoadCamera.transform.localPosition = levelLoadInfo.levelLoadCameraPosition;
        this.gameObject.transform.position = new Vector3(-levelLoadCamera.transform.position.x, this.gameObject.transform.position.y, -levelLoadCamera.transform.position.z);
        mapClouds.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", levelLoadInfo.mapCloudsOffset);
        mapLowClouds.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", levelLoadInfo.mapLowCloudsOffset);
        scriptStage = 1;
    }

	void Start () 
    {
        LevelLoader.GetLevelPresetting(this);
	}
	
	void LateUpdate () 
    {
        if (scriptStage == 1)
        {
            Vector3 targetPosition = mainCamera.gameObject.transform.position;
            if (((targetPosition - levelLoadCamera.gameObject.transform.position).normalized * Time.deltaTime * 15).magnitude < (targetPosition /*+ displacementMotionVector*/ - levelLoadCamera.gameObject.transform.position).magnitude)
            {
                levelLoadCamera.gameObject.transform.position = levelLoadCamera.gameObject.transform.position + (targetPosition + /*displacementMotionVector*/ -levelLoadCamera.gameObject.transform.position).normalized * Time.deltaTime * 15;
            }
            else
            {
                levelLoadCamera.gameObject.transform.position = targetPosition;
                scriptStage = 2;
            }
        }
        if (scriptStage == 2)
        {
            {
                float angle = Vector3.Angle(mainCamera.gameObject.transform.forward - new Vector3(mainCamera.gameObject.transform.forward.x, 0, 0), Vector3.forward);
                if (Vector3.Angle(mainCamera.gameObject.transform.forward - new Vector3(mainCamera.gameObject.transform.forward.x, 0, 0), Vector3.up) < 90f)
                {
                    angle = 360 - angle;
                }
                float angle2 = Vector3.Angle(levelLoadCamera.gameObject.transform.forward - new Vector3(levelLoadCamera.gameObject.transform.forward.x, 0, 0), Vector3.forward);
                if (Vector3.Angle(levelLoadCamera.gameObject.transform.forward - new Vector3(levelLoadCamera.gameObject.transform.forward.x, 0, 0), Vector3.up) < 90f)
                {
                    angle2 = 360 - angle2;
                }
                float angleRotation = +angle - angle2;
                if (angleRotation < -180)
                {
                    angleRotation += 360;
                }
                if (angleRotation > 180)
                {
                    angleRotation -= 360;
                }

                if (Mathf.Abs(angleRotation) > 0.1)
                {
                    float rotationPerUpdate = 5 * Time.deltaTime;
                    if (rotationPerUpdate > Mathf.Abs(angleRotation))
                        rotationPerUpdate = Mathf.Abs(angleRotation);
                    levelLoadCamera.gameObject.transform.Rotate(levelLoadCamera.gameObject.transform.right, +Mathf.Sign(angleRotation) * rotationPerUpdate);
                }
                else
                {
                    scriptStage = 3;
                }
            }
        }

        if (scriptStage == 3)
        {
            LevelLoader.LevelLoadScriptCompleted = true;
            foreach (GameObject go in inactiveObjects)
            {
                go.SetActive(true);
            }
            levelLoadCamera.SetActive(false);
            Destroy(this.gameObject);
            scriptStage++;
        }
	
	}
}
