using UnityEngine;

public class PlayerWorldSelection : MonoBehaviour
{
    private PropertyBase selectedProperty;
    public void UpdateSelection()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (!hit.transform.TryGetComponent(out PropertyBase property))
            {
                TurnOffSelectedProperty();
                return;
            }
            property.TogglePropUI(true);
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
        selectedProperty.TogglePropUI(false);
        selectedProperty = null;
    }
}
