using UnityEngine;
using System.Collections;

public class InputManager : UnitySingleton<InputManager> 
{
    public GameUserInterface gameUserInterface;
    public UpgradeUserInterface upgradeUserInterface;
    public FinalResultInterface finalResultInterface;

    protected override void Awake()
    {
        base.Awake();
        gameUserInterface.inputManager = this;
        upgradeUserInterface.inputManager = this;
    }

    public void ActivateFinalResultInterface(bool isWinner)
    {
        gameUserInterface.gameObject.SetActive(false);
        finalResultInterface.gameObject.SetActive(true);
        finalResultInterface.ShowResult(isWinner);
        Time.timeScale = 0;
    }

    public void ActivateUpgradeInterface()
    {
        gameUserInterface.gameObject.SetActive(false);
        SteamcopterManager.Instance.steamcopterRigidbody.velocity = new Vector3(0, 0, 0);
        SteamcopterManager.Instance.steamcopterRigidbody.angularVelocity = new Vector3(0, 0, 0);
        GameCameraScript.Instance.ActivateUpgradeMode();
        upgradeUserInterface.gameObject.SetActive(true);
        Time.timeScale = 0.00001f;

    }

    public void ActivateGameInterface()
    {
        upgradeUserInterface.gameObject.SetActive(false);
        gameUserInterface.gameObject.SetActive(true);
        GameCameraScript.Instance.ActivateGameMode();
        upgradeUserInterface.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
