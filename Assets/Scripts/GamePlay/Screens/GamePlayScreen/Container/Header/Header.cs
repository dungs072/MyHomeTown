using System.Collections;
using BaseEngine;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace GamePlayContainerElements
{
    public class Header : MonoBehaviour
    {
        [SerializeField] private MagicButton settingButton;
        [SerializeField] private TMP_Text moneyText;

        private Tween moneyTextTween;

        void Awake()
        {
            PlayerWallet.OnMoneyChanged += SetMoneyText;
        }
        void OnDestroy()
        {
            PlayerWallet.OnMoneyChanged -= SetMoneyText;
        }

        public void RegisterSettingButtonEvent(System.Action action)
        {
            settingButton.AddListener(() => action?.Invoke());
        }

        public void SetMoneyText(int money)
        {
            var currentNumber = int.TryParse(moneyText.text, out var parsedNumber) ? parsedNumber : 0;
            if (currentNumber == money)
            {
                moneyText.text = money.ToString();
                return;
            }
            else
            {
                moneyText.text = currentNumber.ToString();
            }
            if (moneyTextTween != null && moneyTextTween.IsActive())
            {
                moneyTextTween.Kill();
            }
            moneyTextTween = AnimUtils.PlayNumberCounterAnim(moneyText,
                int.Parse(moneyText.text),
                money,
                0.5f);
        }

    }

}
