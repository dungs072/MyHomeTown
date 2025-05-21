using System.Collections;
using BaseEngine;
using static GameController;
public class GamePlayScene : BaseScene
{
    public void HandleContinueGame()
    {
        StartCoroutine(HandleContinueGameAsync());
    }
    private IEnumerator HandleContinueGameAsync()
    {
        string screenName = ScreenName.SettingScreen.ToString();
        GameInstance.BlockInput(true);
        yield return ScreenManager.CloseScreenAsync(screenName);
        GameInstance.BlockInput(false);
    }
    public void HandleOpenSettingScreen()
    {
        StartCoroutine(HandleOpenSettingScreenAsync());
    }
    private IEnumerator HandleOpenSettingScreenAsync()
    {
        GameInstance.BlockInput(true);
        string screenName = ScreenName.SettingScreen.ToString();
        yield return ScreenManager.OpenScreenAsync(screenName);
        GameInstance.BlockInput(false);
    }
}
