using UnityEngine;
using System.Collections;

public class InformationAboutLevelPanelScript : MonoBehaviour 
{
    public MapMenu mapMenu;
    public tk2dUIItem levelFlag;
    public tk2dUIItem backToMapMenuButton;
    public tk2dUIItem playLevelButton;

    void OnEnable()
    {
        backToMapMenuButton.OnClickUIItem += BackToMapMenuButtonPressed;
        playLevelButton.OnClickUIItem += PlayLevelButtonPressed;
    }

    void OnDisable()
    {
        backToMapMenuButton.OnClickUIItem -= BackToMapMenuButtonPressed;
        playLevelButton.OnClickUIItem -= PlayLevelButtonPressed;
    }

    void PlayLevelButtonPressed(tk2dUIItem Tk2dUIItem)
    {
        mapMenu.ActivateLoadLevelScript(levelFlag);
    }

    void BackToMapMenuButtonPressed(tk2dUIItem Tk2dUIItem)
    {
        mapMenu.ReturnToMapMenu();
    }
}
