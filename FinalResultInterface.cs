using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinalResultInterface : MonoBehaviour 
{
    public tk2dSlicedSprite mainText;
    public tk2dUIItem restartButton;
    public tk2dUIItem exitButton;
    public tk2dTextMesh killedInfoPanel;
    public tk2dTextMesh resourceInfoPanel;
    public tk2dTextMesh timeInfoPanel;

    void Awake()
    {
        restartButton.OnClickUIItem += RestartButtonPressed;
        exitButton.OnClickUIItem += ExitButtonPressed;
    }

    public void ShowResult(bool isWinner)
    {
        if (isWinner)
        {
            mainText.SetSprite(1);
        }
        else
        {
            mainText.SetSprite(2);
        }
        killedInfoPanel.text = killedInfoPanel.text + " " + PlayerInfoManager.KilledEnemies.ToString();
        killedInfoPanel.Commit();
        resourceInfoPanel.text = resourceInfoPanel.text + " " + PlayerInfoManager.resourcesReceived.ToString();
        resourceInfoPanel.Commit();
        timeInfoPanel.text = timeInfoPanel.text + " " + Time.time.ToString();
        timeInfoPanel.Commit();
    }

    void RestartButtonPressed(tk2dUIItem Tk2dUIItem)
    {
        PlayerInfoManager.LoadPlayerInfo();
        Time.timeScale = 1;
        StartCoroutine(LevelLoader.LoadLevelAsync());
    }

    void ExitButtonPressed(tk2dUIItem Tk2dUIItem)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");
    }


}
