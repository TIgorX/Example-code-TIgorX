using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public static class LevelLoader
{
    private static LevelLoadInfo levelLoadInfo;

    public static bool LevelLoadScriptCompleted = false;
    public static bool LevelTrainingScriptCompleted = false;
    public static bool loadScriptPreparationDone = false;

    public static IEnumerator LoadLevelAsync()
    {
        LevelLoadScriptCompleted = false;
        loadScriptPreparationDone = false;

        if (LevelTrainingScriptCompleted == true)
        {
            loadScriptPreparationDone = true;
        }
        AsyncOperation async = SceneManager.LoadSceneAsync("GameScene1");
            yield return async;
    }

    public static void SetLevelPresetting(LevelLoadInfo _levelLoadInfo)
    {
        levelLoadInfo = _levelLoadInfo;
    }

    public static void GetLevelPresetting(LevelLoadScript levelLoadScript)
    {
        levelLoadScript.SetLevelPresetting(levelLoadInfo);
    }
}

public class LevelLoadInfo
{
    public Vector3 levelLoadCameraPosition;
    public Vector2 mapCloudsOffset;
    public Vector2 mapLowCloudsOffset;

    public LevelLoadInfo(Vector3 LevelLoadCameraPosition, Vector2 MapCloudsOffset, Vector2 MapLowCloudsOffset)
    {
        levelLoadCameraPosition = LevelLoadCameraPosition;
        mapCloudsOffset = MapCloudsOffset;
        mapLowCloudsOffset = MapLowCloudsOffset;
    }
}
