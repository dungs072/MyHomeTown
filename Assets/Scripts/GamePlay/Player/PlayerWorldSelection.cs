using UnityEngine;

public class PlayerWorldSelection : CoreBehavior
{
    private PropertyBase selectedProperty;

    protected override void OnEnableBehavior()
    {
        base.OnEnableBehavior();
        PlayerInput.OnSelectionObject += OnSelectionObject;
    }
    protected override void OnDisableBehavior()
    {
        base.OnDisableBehavior();
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
