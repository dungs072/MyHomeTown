using UnityEngine;
using TMPro;
public class InfoPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text stateText;


    void Awake()
    {
        // disable the info panel by default
        gameObject.SetActive(false);
    }

    public void SetNameText(string name)
    {
        if (nameText != null)
        {
            nameText.text = "Name: " + name;
        }
    }
    public void SetStateText(string state)
    {
        if (stateText != null)
        {
            stateText.text = "State: " + state;
        }
    }
}
