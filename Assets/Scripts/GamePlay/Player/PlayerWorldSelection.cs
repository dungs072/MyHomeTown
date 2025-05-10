using UnityEngine;

public class PlayerWorldSelection : MonoBehaviour
{
    private PropertyBase selectedProperty;

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
            if (!hit.transform.TryGetComponent(out PropertyBase property))
            {
                TurnOffSelectedProperty();
                return;
            }
            property.SetPropState(PropState.Selected);
            selectedProperty = property;
        }
        else
        {
            TurnOffSelectedProperty();
        }

    }
    private void TurnOffSelectedProperty()
    {
        if (selectedProperty == null) return;
        selectedProperty.SetPropState(PropState.Free);
        selectedProperty = null;
    }
}
