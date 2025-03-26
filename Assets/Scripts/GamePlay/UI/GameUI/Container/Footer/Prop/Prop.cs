using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Prop : MonoBehaviour
{
    [SerializeField] private Image background;

    [SerializeField] private TMP_Text textName;


    public void SetName(string name)
    {
        textName.text = name;
    }
    public void SetBackground(Sprite sprite)
    {
        background.sprite = sprite;
    }
}
