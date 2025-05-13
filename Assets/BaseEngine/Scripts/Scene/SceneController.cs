using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }
    private string currentSceneName;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

    }
    public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        currentSceneName = sceneName;
        SceneManager.LoadScene(sceneName, mode);
    }

    public void LoadScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single)
    {
        SceneManager.LoadScene(sceneIndex, mode);
    }

    public static void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }


    public IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, mode);
        while (!asyncOp.isDone)
        {
            yield return null;
        }
        currentSceneName = sceneName;
    }
}
