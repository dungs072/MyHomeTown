using System;
using BaseEngine;
using UnityEngine;
using UnityEngine.UI;

public class DashboardContainer : MonoBehaviour
{
    [SerializeField] private MagicButton playGameButton;
    [SerializeField] private MagicButton settingsButton;
    [SerializeField] private MagicButton exitButton;


    public void AddActionToPlayGameButton(Action action)
    {
        playGameButton.AddListener(() => action?.Invoke());
    }
    public void AddActionToSettingsButton(Action action)
    {
        settingsButton.AddListener(() => action?.Invoke());
    }
    public void AddActionToExitButton(Action action)
    {
        exitButton.AddListener(() => action?.Invoke());
    }
}
