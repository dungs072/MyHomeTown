using UnityEngine;
using TMPro;
public class InfoPersonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;

    public void SetInfoText(string text)
    {
        infoText.text = text;
    }
}
