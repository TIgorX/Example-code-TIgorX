using UnityEngine;
using System.Collections;

public class EndLevelScript : MonoBehaviour 
{
    public GameObject gateObject;
    public GameUserInterface gameUserInterface;
    public GameObject gameCamera;
    public GameObject targetMotionObject;

    private bool isTarget = false;
    private float StartedAnimateLevelScriptTime = -1;
    private bool StartedAnimateLevelScript = false;
    private bool EndedAnimateLevelScript = false;

    public void StartScript()
    {
        gameUserInterface.gameObject.SetActive(false);
        isTarget = true;

        this.gameObject.transform.position = gameCamera.transform.position;
        this.gameObject.transform.rotation = gameObject.transform.rotation;
        gameCamera.SetActive(false);
        this.gameObject.SetActive(true);
    }

    void Update()
    {
        if ((!StartedAnimateLevelScript) &&
            ((targetMotionObject.transform.position - this.gameObject.transform.position).magnitude <= 5))
        {
            StartedAnimateLevelScript = true;
            StartedAnimateLevelScriptTime = Time.time;
        }

        if ((StartedAnimateLevelScript) && (!EndedAnimateLevelScript))
        {
            gateObject.transform.Translate(new Vector3(6f * Time.deltaTime, 0,0), Space.Self);
        }

        if ((StartedAnimateLevelScript) && (!EndedAnimateLevelScript) &&
            ((StartedAnimateLevelScriptTime + 4) <= Time.time))
        {
            EndedAnimateLevelScript = true;
            AnimateLevelScript();
        }

    }

    private void AnimateLevelScript()
    {
        InputManager.Instance.ActivateFinalResultInterface(true);
    }
}
