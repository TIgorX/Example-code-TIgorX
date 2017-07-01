using UnityEngine;
using System.Collections;

public class GUISupportScript : MonoBehaviour {

    private Camera gameCamera;
    private EnemyAIBasic eAIBasic;

    void Awake()
    {
        eAIBasic = this.gameObject.GetComponent<EnemyAIBasic>();
    }

	void Start () 
    {
        gameCamera = GameCameraScript.Instance.gameObject.GetComponent<Camera>();
	}
	
    void OnGUI()
    {
        GUIStyle guis = new GUIStyle(GUIStyle.none);
        guis.normal.textColor = Color.green;
        Vector3 positionOnScreen = (gameCamera.WorldToScreenPoint(this.gameObject.transform.position));
        GUI.Label(new Rect(positionOnScreen.x, Screen.height - positionOnScreen.y - 50, 50, 50), eAIBasic.currentHealth.ToString(), guis);
    }
}
