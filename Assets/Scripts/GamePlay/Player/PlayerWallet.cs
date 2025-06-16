using System;
using BaseEngine;
using UnityEngine;
using static GameController;

public class PlayerWallet : MonoBehaviour
{
    public static Action<int> OnMoneyChanged;
    public int Money => GameInstance.GameStorage.GetMoney();
    void Awake()
    {
        BaseScreen.OnScreenOpened += OnScreenOpened;
    }
    void OnDestroy()
    {
        BaseScreen.OnScreenOpened -= OnScreenOpened;
    }

    //! Temporarily. Make sure to Update UI when game play screen is created.
    private void OnScreenOpened(BaseScreen screen)
    {
        if (screen is not GamePlayScreen) return;
        SetUp();
    }

    private void SetUp()
    {
        var screenName = ScreenName.GamePlayScreen.ToString();
        var gamePlayScreen = GameInstance.ScreenManager.GetScreen<GamePlayScreen>(screenName);
        gamePlayScreen.Container.Header.SetMoneyText(Money);
        AddMoney(PlayerConfig.START_MONEY);
    }

    public void AddMoney(int amount)
    {
        int currentMoney = GameInstance.GameStorage.GetMoney();
        var totalMoney = currentMoney + amount;
        GameInstance.GameStorage.SaveMoney(totalMoney);
        OnMoneyChanged?.Invoke(totalMoney);
    }
    public bool CanBuy(int price)
    {
        int currentMoney = GameInstance.GameStorage.GetMoney();
        return currentMoney >= price;
    }
}
