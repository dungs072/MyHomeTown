using UnityEngine;


[RequireComponent(typeof(Occupier))]
public class PropertyBase : BaseProduct
{
    [SerializeField] private PropKeys propKey = PropKeys.CookingBase;

    public override string ProductName => PropKeyDescriptions.Descriptions[propKey];


}
