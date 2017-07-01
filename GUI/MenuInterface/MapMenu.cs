using UnityEngine;
using System.Collections;

public class MapMenu : MonoBehaviour 
{
    public MapPerspectiveCameraScript mapPerspectiveCamera;
    public tk2dCamera orthographicCamera;
    public InformationAboutLevelPanelScript informationAboutLevelPanel;

    public Material mapClouds;
    public Material mapLowClouds;
    public tk2dUIItem[] levelFlags;
    public tk2dUIItem backToMainMenuButton;

    void Awake()
    {
        informationAboutLevelPanel.mapMenu = this;
    }

    void OnEnable()
    {
        SubscribeLevelFlags();
        backToMainMenuButton.OnClickUIItem += BackToMainMenuButtonPressed;
    }

    void OnDisable()
    {
        UnsubscribeLevelFlags();
        backToMainMenuButton.OnClickUIItem -= BackToMainMenuButtonPressed;
    }

    private void SubscribeLevelFlags()
    {
        foreach (tk2dUIItem tk2duii in levelFlags)
        {
            tk2duii.OnClickUIItem += LevelFlagPressed;
        }
    }

    private void UnsubscribeLevelFlags()
    {
        foreach (tk2dUIItem tk2duii in levelFlags)
        {
            tk2duii.OnClickUIItem -= LevelFlagPressed;
        }
    }

    public void ReturnToMapMenu()
    {
        informationAboutLevelPanel.gameObject.SetActive(false);
        SubscribeLevelFlags();
        backToMainMenuButton.OnClickUIItem += BackToMainMenuButtonPressed;   
    }

    void LevelFlagPressed(tk2dUIItem Tk2dUIItem)
    {
        informationAboutLevelPanel.gameObject.SetActive(true);
        informationAboutLevelPanel.levelFlag = Tk2dUIItem;
        UnsubscribeLevelFlags();
        backToMainMenuButton.OnClickUIItem -= BackToMainMenuButtonPressed;   
    }

    void BackToMainMenuButtonPressed(tk2dUIItem Tk2dUIItem)
    {
        ManagerMainMenu.Instance.ActivateMainMenu();
    }

    public void ActivateLoadLevelScript(tk2dUIItem Tk2dUIItem)
    {
        informationAboutLevelPanel.gameObject.SetActive(false);
        orthographicCamera.gameObject.SetActive(false);
        mapPerspectiveCamera.SetTarget(Tk2dUIItem.gameObject);
    }
}
