using UnityEngine;


[RequireComponent(typeof(Occupier))]
public class PropertyBase : BaseProduct
{
    [SerializeField] private PropKeys propKey = PropKeys.CookingBase;
    [SerializeField] private PropUI propUI;
    public override string ProductName => PropKeyDescriptions.Descriptions[propKey];

    void Start()
    {
        TogglePropUI(false);
    }

    public void TogglePropUI(bool isActive)
    {
        if (!propUI) return;
        propUI.gameObject.SetActive(isActive);
    }

}
