using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    private ScreenManager screenManager;
    void Awake()
    {
        InitSingleton();
        InitCustomGameEngineComponents();
    }

    private void InitSingleton()
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

    private void InitCustomGameEngineComponents()
    {
        screenManager = ScreenManager.Instance;
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
        yield return OpenGameScreen();
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

    public IEnumerator OpenGameScreen()
    {
        string screenName = ScreenName.GamePlayScreen.ToString();
        yield return ScreenManager.OpenScreenAsync(screenName);
    }


}
