using System;
using BaseEngine;
using UnityEngine;
namespace SettingContainerElements
{
    public class Middle : MonoBehaviour
    {
        [SerializeField] private MagicButton continueButton;
        [SerializeField] private MagicButton optionsButton;
        [SerializeField] private MagicButton exitMenuButton;
        [SerializeField] private MagicButton exitGameButton;

        public void AddActionToContinueButton(Action action)
        {
            continueButton.AddListener(() => action?.Invoke());
        }
        public void AddActionToOptionsButton(Action action)
        {
            optionsButton.AddListener(() => action?.Invoke());
        }
        public void AddActionToExitMenuButton(Action action)
        {
            exitMenuButton.AddListener(() => action?.Invoke());
        }
        public void AddActionToExitGameButton(Action action)
        {
            exitGameButton.AddListener(() => action?.Invoke());
        }
    }

}
