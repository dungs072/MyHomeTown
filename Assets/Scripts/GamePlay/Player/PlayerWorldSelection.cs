using UnityEngine;

public class PlayerWorldSelection : MonoBehaviour
{
    //private PropertyBase selectedProperty;
    private SelectableObject selectedObject;
    void OnEnable()
    {
        PlayerInput.OnSelectionObject += OnSelectionObject;

    }


    void OnDisable()
    {
        PlayerInput.OnSelectionObject -= OnSelectionObject;
    }

    private void OnSelectionObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TurnOffSelectedObject();
            if (!hit.transform.TryGetComponent(out SelectableObject obj)) return;
            selectedObject = obj;
            selectedObject.SetSelected(true);
        }
        else
        {
            TurnOffSelectedObject();
        }
    }
    private void TurnOffSelectedObject()
    {
        if (selectedObject == null) return;
        selectedObject.SetSelected(false);
        selectedObject = null;
    }
}
