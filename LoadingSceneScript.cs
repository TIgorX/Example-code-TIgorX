using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingSceneScript : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
