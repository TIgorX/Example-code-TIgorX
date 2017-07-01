using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
    public tk2dUIItem playButton;
    public tk2dSlicedSprite playButtonGraphic;

    void Awake()
    {
        playButton.OnClickUIItem += PlayButtonPressed;
    }

	void Update () 
    {
        playButtonGraphic.color = new Color(1, 1, 1, (playButtonGraphic.color.a + 0.75f*Time.deltaTime ) % 1);
	}

    void PlayButtonPressed(tk2dUIItem Tk2dUIItem)
    {
        PlayerInfoManager.LoadPlayerInfo();
        ManagerMainMenu.Instance.ActivateMapMenu();
    }
}
