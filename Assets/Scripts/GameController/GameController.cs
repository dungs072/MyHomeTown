using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    void Awake()
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

    void Start()
    {
        StartCoroutine(StartGame());
    }
    private IEnumerator StartGame()
    {
        yield return OpenDashboardScene();
        yield return new WaitForSeconds(1f);
        yield return OpenGameScene();
    }
    public IEnumerator OpenDashboardScene()
    {
        string sceneName = SceneUtils.GetSceneName(SceneKey.DashboardScene);
        yield return SceneController.LoadSceneAsync(sceneName, LoadSceneMode.Single);

    }
    public IEnumerator OpenGameScene()
    {
        string sceneName = SceneUtils.GetSceneName(SceneKey.GameScene);
        yield return SceneController.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }


}
