using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Prop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image background;

    [SerializeField] private TMP_Text textName;

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
        textName.text = name;
        propName = name;
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
        OnStartCreatePropAction?.Invoke();
        StartCoroutine(handlePointerDrag());
    }
    public IEnumerator handlePointerDrag()
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
