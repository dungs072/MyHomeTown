using BaseEngine;
using UnityEngine;

namespace GamePlayContainerElements
{
    public class Header : MonoBehaviour
    {
        [SerializeField] private MagicButton settingButton;

        public void RegisterSettingButtonEvent(System.Action action)
        {
            settingButton.AddListener(() => action?.Invoke());
        }
    }

}
