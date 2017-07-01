using UnityEngine;
using System.Collections;

public class MapPerspectiveCameraScript : MonoBehaviour
{
    public GameObject mapClouds;
    public GameObject mapLowClouds;

    private Vector3 targetPosition;
    private GameObject targetObject;
    private bool isTarget = false;
    public int scriptStage = 0;
    private float startScriptStage3 = 0;

    private Vector3 displacementMotionVector = new Vector3(0, -3, -6);
    private Vector3 displacementFocusVector = new Vector3(0, -10, -6);

    public void SetTarget(GameObject TargetObject)
    {
        targetObject = TargetObject;
        targetPosition = TargetObject.transform.position;
        isTarget = true;
        scriptStage = 1;
    }

    void Awake()
    {
        mapClouds.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(Random.Range(-2, 2f), Random.Range(-2, 2f)));
        mapLowClouds.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(Random.Range(-2, 2f), Random.Range(-2, 2f)));
    }

    void LateUpdate()
    {
        if (isTarget)
        {
            if (scriptStage == 1) 
            {
                if (((targetPosition + displacementMotionVector - this.gameObject.transform.position).normalized * Time.deltaTime * 7).magnitude < (targetPosition + displacementMotionVector - this.gameObject.transform.position).magnitude)
                {
                    this.gameObject.transform.position = this.gameObject.transform.position + (targetPosition + displacementMotionVector - this.gameObject.transform.position).normalized * Time.deltaTime * 7;
                }
                else
                {
                    this.gameObject.transform.position = targetPosition + displacementMotionVector;
                }

                Vector3 focusVector = (targetPosition + displacementFocusVector - this.gameObject.transform.position);

                {
                    float angle = Vector3.Angle(focusVector - new Vector3(0, focusVector.y, 0), Vector3.forward);
                    if (Vector3.Angle(focusVector - new Vector3(0, focusVector.y, 0), Vector3.left) < 90f)
                    {
                        angle = 360 - angle;
                    }
                    float angle2 = Vector3.Angle(this.gameObject.transform.forward - new Vector3(0, this.gameObject.transform.forward.y, 0), Vector3.forward);
                    if (Vector3.Angle(this.gameObject.transform.forward - new Vector3(0, this.gameObject.transform.forward.y, 0), Vector3.left) < 90f)
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
                        float rotationPerUpdate = 20 * Time.deltaTime;
                        if (rotationPerUpdate > Mathf.Abs(angleRotation))
                            rotationPerUpdate = Mathf.Abs(angleRotation);
                        this.gameObject.transform.Rotate(this.gameObject.transform.forward, -Mathf.Sign(angleRotation) * rotationPerUpdate);
                    }
                }

                {
                    float angle = Vector3.Angle(focusVector - new Vector3(focusVector.x, 0, 0), Vector3.forward);
                    if (Vector3.Angle(focusVector - new Vector3(focusVector.x, 0, 0), Vector3.up) < 90f)
                    {
                        angle = 360 - angle;
                    }
                    float angle2 = Vector3.Angle(this.gameObject.transform.forward - new Vector3(this.gameObject.transform.forward.x, 0, 0), Vector3.forward);
                    if (Vector3.Angle(this.gameObject.transform.forward - new Vector3(this.gameObject.transform.forward.x, 0, 0), Vector3.up) < 90f)
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
                        float rotationPerUpdate = 15 * Time.deltaTime;
                        if (rotationPerUpdate > Mathf.Abs(angleRotation))
                            rotationPerUpdate = Mathf.Abs(angleRotation);
                        this.gameObject.transform.Rotate(this.gameObject.transform.right, +Mathf.Sign(angleRotation) * rotationPerUpdate);
                    }
                }
                if ((targetPosition + displacementMotionVector - this.gameObject.transform.position).magnitude <= 0.01)
                {
                    scriptStage = 2;
                    AnimateLevelFlag();
                }
            }

            if (scriptStage == 2)
            {
                {
                    float angle2 = Vector3.Angle(this.gameObject.transform.up - new Vector3(0, this.gameObject.transform.up.y, 0), Vector3.forward);
                    if (Vector3.Angle(this.gameObject.transform.up - new Vector3(0, this.gameObject.transform.up.y, 0), Vector3.left) < 90f)
                    {
                        angle2 = 360 - angle2;
                    }
                    float angleRotation = +0 - angle2;
                    if (angleRotation < -180)
                    {
                        angleRotation += 360;
                    }

                    if (angleRotation > 180)
                    {
                        angleRotation -= 360;
                    }
                    if (Mathf.Abs(angleRotation) > 1)
                    {
                        float rotationPerUpdate = 20 * Time.deltaTime;
                        if (rotationPerUpdate > Mathf.Abs(angleRotation))
                            rotationPerUpdate = Mathf.Abs(angleRotation);
                        this.gameObject.transform.Rotate(this.gameObject.transform.up, -Mathf.Sign(angleRotation) * rotationPerUpdate);
                    }
                    else
                    {
                        scriptStage = 3;
                        startScriptStage3 = Time.time;
                    }
                }
            }

            if (scriptStage == 3)
            {
                if ((startScriptStage3 + 0.5f) < Time.time)
                {
                    scriptStage = 4;
                }
            }

            if (scriptStage == 4)
            {
                LevelLoader.SetLevelPresetting(
                    new LevelLoadInfo(this.gameObject.transform.localPosition, mapClouds.GetComponent<Renderer>().material.GetTextureOffset("_MainTex"), mapLowClouds.GetComponent<Renderer>().material.GetTextureOffset("_MainTex")));

                StartCoroutine(LevelLoader.LoadLevelAsync());
                scriptStage = 5;
            }

            if (scriptStage == 5)
            {
                LevelLoader.SetLevelPresetting(
                    new LevelLoadInfo(this.gameObject.transform.localPosition, mapClouds.GetComponent<Renderer>().material.GetTextureOffset("_MainTex"), mapLowClouds.GetComponent<Renderer>().material.GetTextureOffset("_MainTex")));
            }
        }
    }

    private void AnimateLevelFlag()
    {
    }
}
