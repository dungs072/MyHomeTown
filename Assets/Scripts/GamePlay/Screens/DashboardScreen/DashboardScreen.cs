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
        container.AddActionToPlayGameButton(HandlePlayGameButtonClicked);
        container.AddActionToSettingsButton(HandleSettingsButtonClicked);
        container.AddActionToExitButton(HandleExitButtonClicked);
    }

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


}
