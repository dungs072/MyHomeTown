using System;
using UnityEngine;
using UnityEngine.UI;

public class DashboardContainer : MonoBehaviour
{
    [SerializeField] private Button playGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;


    public void AddActionToPlayGameButton(Action action)
    {
        playGameButton.onClick.AddListener(() => action?.Invoke());
    }
    public void AddActionToSettingsButton(Action action)
    {
        settingsButton.onClick.AddListener(() => action?.Invoke());
    }
    public void AddActionToExitButton(Action action)
    {
        exitButton.onClick.AddListener(() => action?.Invoke());
    }
}
