using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public static class SceneController
{
    public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        SceneManager.LoadScene(sceneName, mode);
    }

    public static void LoadScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single)
    {
        SceneManager.LoadScene(sceneIndex, mode);
    }

    public static void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }


    public static IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, mode);
        while (!asyncOp.isDone)
        {
            yield return null;
        }
    }
}
