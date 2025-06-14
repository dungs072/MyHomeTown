
using UnityEngine;

public class GameStorage
{
    private readonly string MONEY_KEY = "Money";
    public GameStorage()
    {
        // Do something there
    }

    public void SaveMoney(int money)
    {
        PlayerPrefs.SetInt(MONEY_KEY, money);
        PlayerPrefs.Save();
    }
    public int GetMoney()
    {
        return PlayerPrefs.GetInt(MONEY_KEY, 0);
    }
    // add game data here
}