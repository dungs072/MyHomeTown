using UnityEngine;

public enum PropState
{
    Selected,
    Using,
    Free
}

[RequireComponent(typeof(Occupier))]
public class PropertyBase : BaseProduct
{
    [SerializeField] private PropUI propUI;
    [SerializeField] private PropData propData;

    public override string ProductName => propData.PropName;
    private PropState currentState = PropState.Free;
    public PropData PropData => propData;
    private Occupier occupier;
    void Awake()
    {
        PlayerInput.OnRotateObject += OnRotateObject;
        occupier = GetComponent<Occupier>();
    }
    void OnDestroy()
    {
        PlayerInput.OnRotateObject -= OnRotateObject;
    }
    void Start()
    {
        TogglePropUI(false);
    }

    private void OnRotateObject()
    {
        if (currentState != PropState.Selected) return;
        transform.Rotate(Vector3.up, 90f);
        occupier.SwapWidthAndHeight();
        occupier.HandleMovingOnGrid(transform.position);
    }

    public void TogglePropUI(bool isActive)
    {
        if (!propUI) return;
        propUI.gameObject.SetActive(isActive);
    }
    public void SetPropState(PropState state)
    {
        currentState = state;
        if (state == PropState.Using)
        {
            HandleOnUsing();
        }
        else if (state == PropState.Selected)
        {
            HandleOnSelected();
        }
        else if (state == PropState.Free)
        {
            HandleOnFree();
        }
    }
    private void HandleOnUsing()
    {
        TogglePropUI(false);
    }
    private void HandleOnSelected()
    {
        TogglePropUI(true);
    }
    private void HandleOnFree()
    {
        TogglePropUI(false);
    }
}
