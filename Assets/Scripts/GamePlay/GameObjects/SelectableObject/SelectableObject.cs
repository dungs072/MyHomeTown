using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableObject : MonoBehaviour
{
    [SerializeField] private GameObject selectionObject;
    private bool isSelected = false;


    void Awake()
    {
        // force the selection object to be inactive at the start
        selectionObject.SetActive(isSelected);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        selectionObject.SetActive(selected);
    }
}
