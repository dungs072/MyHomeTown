using System.Collections;
using BaseEngine;
using UnityEngine;
using static GameController;
public class DashboardScreen : BaseScreen
{
    [SerializeField] private DashboardContainer container;
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
        middle.AddActionToPlayGameButton(HandlePlayGameButtonClicked);
        middle.AddActionToSettingsButton(HandleSettingsButtonClicked);
        middle.AddActionToExitButton(HandleExitButtonClicked);
    }
    #region Button Events

    private void HandlePlayGameButtonClicked()
    {
        GameInstance.PlayGame();
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
