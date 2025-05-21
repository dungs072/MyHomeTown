using System.Collections;
using BaseEngine;
using UnityEngine;
using static GameController;
using static ManagerSingleton;
public class SettingScreen : BaseScreen
{
    [SerializeField] private SettingContainer container;

    public override IEnumerator OpenScreenAsync()
    {
        yield return base.OpenScreenAsync();
    }

    public override IEnumerator CloseScreenAsync()
    {
        return base.CloseScreenAsync();
    }

    void Start()
    {
        RegisterButtonActions();
    }
    private void RegisterButtonActions()
    {
        var middle = container.Middle;
        middle.AddActionToContinueButton(HandleContinueButtonClicked);
        middle.AddActionToOptionsButton(HandleOptionsButtonClicked);
        middle.AddActionToExitMenuButton(HandleExitMenuButtonClicked);
        middle.AddActionToExitGameButton(HandleExitGameButtonClicked);
    }
    #region Button Events

    private void HandleContinueButtonClicked()
    {
        var gameScene = GameInstance.GetSpecificScene<GamePlayScene>();
        gameScene.HandleContinueGame();
    }
    private void HandleOptionsButtonClicked()
    {
        // Handle options button click
    }
    private void HandleExitMenuButtonClicked()
    {
        GameInstance.ExitDashboard();
    }
    private void HandleExitGameButtonClicked()
    {
        Application.Quit();
    }
    #endregion
}
