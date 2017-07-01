using UnityEngine;
using System.Collections;

public class ManagerMainMenu : UnitySingleton<ManagerMainMenu> 
{
    public MainMenu mainMenu;
    public MapMenu mapMenu;

    protected override void Awake()
    {
        base.Awake();
    }

    public void ActivateMainMenu()
    {
        mapMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }

    public void ActivateMapMenu()
    {
        mainMenu.gameObject.SetActive(false);
        mapMenu.gameObject.SetActive(true);
    }
}
