using System.Collections;
using BaseEngine;
using UnityEngine;

public class DashboardScreen : BaseScreen
{
    [SerializeField] private DashboardContainer container;

    private GameController gameController;
    public override IEnumerator OpenScreenAsync()
    {
        yield return base.OpenScreenAsync();
    }

    public override IEnumerator CloseScreenAsync()
    {
        return base.CloseScreenAsync();
    }

    public override void InitScreen()
    {
        gameController = GameController.Instance;
    }

    void Start()
    {
        RegisterButtonActions();
    }
    private void RegisterButtonActions()
    {
        var middle = container.Middle;
        middle.AddActionToPlayGameButton(HandlePlayGameButtonClicked);
        middle.AddActionToSettingsButton(HandleSettingsButtonClicked);
        middle.AddActionToExitButton(HandleExitButtonClicked);
    }
    #region Button Events

    private void HandlePlayGameButtonClicked()
    {
        gameController.PlayGame();
    }

    private void HandleSettingsButtonClicked()
    {
        // Handle settings button click
    }
    private void HandleExitButtonClicked()
    {
        Application.Quit();
    }
    #endregion


}
