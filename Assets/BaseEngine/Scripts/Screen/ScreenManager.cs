using System;
using System.Collections.Generic;

public class ScreenManager : IDisposable
{
    public static ScreenManager Instance => new();
    private static Dictionary<string, BaseScreen> screens;
    #region Base

    /*
    Please call this method in the Awake method of the GameController class. 
    */
    public void Init()
    {
        RegisterEvents();
        InitComponents();
    }

    public void Dispose()
    {
        UnregisterEvents();

    }
    private void InitComponents()
    {
        screens = new Dictionary<string, BaseScreen>();
    }
    private void RegisterEvents()
    {
        BaseScreen.OnScreenCreated += OnScreenCreated;
        BaseScreen.OnScreenDestroyed += OnScreenDestroyed;
    }
    private void UnregisterEvents()
    {
        BaseScreen.OnScreenCreated -= OnScreenCreated;
        BaseScreen.OnScreenDestroyed -= OnScreenDestroyed;
    }

    private void OnScreenCreated(BaseScreen screen)
    {
        if (screens.ContainsKey(screen.name))
        {
            Console.WriteLine($"Screen {screen.name} already exists.");
            return;
        }
        screens.Add(screen.name, screen);

    }

    private void OnScreenDestroyed(BaseScreen screen)
    {
        if (!screens.ContainsKey(screen.name))
        {
            Console.WriteLine($"Screen {screen.name} does not exist.");
            return;
        }
        screens.Remove(screen.name);
    }
    #endregion

    #region Utility
    public void OpenScreen(string screenName)
    {
        if (screens.TryGetValue(screenName, out BaseScreen screen))
        {
            screen.OpenScreen();
            return;
        }
        Console.WriteLine($"Screen {screenName} not found.");
    }
    public void CloseScreen(string screenName)
    {
        if (screens.TryGetValue(screenName, out BaseScreen screen))
        {
            screen.CloseScreen();
            return;
        }
        Console.WriteLine($"Screen {screenName} not found.");
    }
    #endregion


}