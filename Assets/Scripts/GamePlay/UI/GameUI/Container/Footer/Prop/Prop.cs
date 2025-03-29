using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Prop : MonoBehaviour
{
    [SerializeField] private Image background;

    [SerializeField] private TMP_Text textName;

    private Button buttonComponent;
    private string propName;
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

    public void RegisterButtonClickEvent(Action action)
    {
        buttonComponent.onClick.AddListener(() => action?.Invoke());
    }
}
