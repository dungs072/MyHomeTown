using System.Collections;
using BaseEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    private ScreenManager screenManager;
    private SceneController sceneController;
    private GameStorage gameStorage;

    public GameStorage GameStorage => gameStorage;
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
        sceneController = SceneController.Instance;
        gameStorage = new GameStorage();
    }

    void Start()
    {
        StartCoroutine(InitGameAsync());
    }
    private IEnumerator InitGameAsync()
    {
        yield return OpenDashboardScene();
        yield return OpenDashboardScreen();
    }

    public void PlayGame()
    {
        StartCoroutine(PlayGameAsync());
    }
    private IEnumerator PlayGameAsync()
    {
        yield return OpenGameScene();
        yield return OpenGameScreen();
    }

    #region Scene
    public IEnumerator OpenDashboardScene()
    {
        string sceneName = SceneUtils.GetSceneName(SceneKey.DashboardScene);
        yield return sceneController.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
    public IEnumerator OpenGameScene()
    {
        string sceneName = SceneUtils.GetSceneName(SceneKey.GameScene);
        yield return sceneController.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }

    #endregion

    #region Screen
    public IEnumerator OpenDashboardScreen()
    {
        string screenName = ScreenName.DashboardScreen.ToString();
        yield return ScreenManager.OpenScreenAsync(screenName);
    }
    public IEnumerator OpenGameScreen()
    {
        string screenName = ScreenName.GamePlayScreen.ToString();
        yield return ScreenManager.OpenScreenAsync(screenName);
    }
    #endregion




}
