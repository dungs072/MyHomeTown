using System.Collections;
using BaseEngine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private GameObject blockInputContainer;
    [Header("Debugger")]
    [SerializeField] private Image blockInputImage;
    public static GameController GameInstance { get; private set; }
    private ScreenManager screenManager;
    private SceneController sceneController;
    private GameStorage gameStorage;

    public GameStorage GameStorage => gameStorage;

    public T GetSpecificScene<T>() where T : class
    {
        var baseScene = sceneController.CurrentScene;
        if (!baseScene) return null;
        return baseScene as T;
    }

    void Awake()
    {
        InitSingleton();
        InitCustomGameEngineComponents();
        // debugger
        UpdateBlockInputContainer();
    }

    private void InitSingleton()
    {
        if (GameInstance == null)
        {
            GameInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (GameInstance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitCustomGameEngineComponents()
    {
        screenManager = ScreenManager.ScreenManagerInstance;
        sceneController = SceneController.Instance;
        gameStorage = new GameStorage();
    }

    void Start()
    {
        StartCoroutine(InitGameAsync());
    }
    private IEnumerator InitGameAsync()
    {
        BlockInput(true);
        yield return OpenDashboardScene();
        yield return OpenDashboardScreen();
        BlockInput(false);
    }

    public void PlayGame()
    {
        StartCoroutine(PlayGameAsync());
    }
    private IEnumerator PlayGameAsync()
    {
        BlockInput(true);
        yield return OpenGameScene();
        yield return OpenGameScreen();
        BlockInput(false);
    }
    public void ExitDashboard()
    {
        StartCoroutine(ExitDashboardAsync());
    }
    private IEnumerator ExitDashboardAsync()
    {
        BlockInput(true);
        yield return CloseSettingScreen();
        yield return OpenDashboardScene();
        yield return OpenDashboardScreen();
        BlockInput(false);
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
    public IEnumerator CloseSettingScreen()
    {
        string screenName = ScreenName.SettingScreen.ToString();
        yield return ScreenManager.CloseScreenAsync(screenName);
    }
    #endregion
    #region Input
    public void BlockInput(bool block)
    {
        if (!blockInputContainer)
        {
            Debug.LogError("BlockInputContainer is not assigned in the inspector.");
            return;
        }
        blockInputContainer.SetActive(block);
    }
    #endregion


    #region Debugger
    private void UpdateBlockInputContainer()
    {
        var alphaTarget = GameConfig.IsDebugMode ? 0.27f : 0;
        var color = blockInputImage.color;
        color.a = alphaTarget;
        blockInputImage.color = color;
    }
    #endregion


}
