using System.Collections.Generic;
using UnityEngine;

public class ScreenCreator : MonoBehaviour
{
    // please add the screen names in order to show the depth of the screen 
    [SerializeField] private List<ScreenName> screenNames;

    void Awake()
    {
        InitScreens();
    }
    private void InitScreens()
    {
        var screenManager = ScreenManager.Instance;
        var screenNameStr = new List<string>();

        screenManager.SetScreenHolder(transform);
        foreach (var screenName in screenNames)
        {
            screenNameStr.Add(screenName.ToString());
        }
        StartCoroutine(screenManager.PreloadScreens(screenNameStr));
    }
}