using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ManagerSingleton;

public class Prop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image background;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;

    private Button buttonComponent;
    private string propName;
    private Action OnStartCreatePropAction;
    private Action OnCreatingPropAction;
    private Action OnFinishCreatePropAction;

    public string PropName => propName;

    void Awake()
    {
        buttonComponent = GetComponent<Button>();
        if (buttonComponent == null)
        {
            Debug.LogError("Button component not found on Prop.");
        }
    }

    public void SetName(string name)
    {
        nameText.text = name;
        propName = name;
    }
    public void SetPrice(int price)
    {
        priceText.text = price.ToString();
    }
    public void SetBackground(Sprite sprite)
    {
        background.sprite = sprite;
    }

    public void RegisterButtonClickEvent(Action start, Action creating, Action finish)
    {
        OnStartCreatePropAction = start;
        OnCreatingPropAction = creating;
        OnFinishCreatePropAction = finish;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        TryToBuyProp();
    }
    private void TryToBuyProp()
    {
        var player = EmpireInstance.Player;
        if (!player.TryGetComponent(out PlayerWallet playerWallet)) return;
        int propPrice = int.Parse(priceText.text);
        if (!playerWallet.CanBuy(propPrice)) return;
        OnStartCreatePropAction?.Invoke();
        StartCoroutine(HandlePointerDrag());

    }
    public IEnumerator HandlePointerDrag()
    {
        while (true)
        {
            OnCreatingPropAction?.Invoke();
            yield return null;
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        OnFinishCreatePropAction?.Invoke();
    }


}
