using System.Collections.Generic;
using UnityEngine;

public class Footer : MonoBehaviour
{
    [SerializeField] private GameObject propHolder;
    [SerializeField] private Prop prop;

    private List<Prop> props = new List<Prop>();

    private PropertyFactory propertyFactory;
    void Awake()
    {
        InitComponents();
    }
    private void InitComponents()
    {
        if (!propertyFactory) return;
        propertyFactory = ManagerSingleton.Instance.PropertyFactory;
    }
    public void CreateProp(string name, Sprite sprite)
    {
        Prop newProp = Instantiate(prop, propHolder.transform);
        props.Add(newProp);
        newProp.SetName(name);
        RegisterEventForPropUI(newProp);
        if (!sprite) return;
        newProp.SetBackground(sprite);
    }

    public void RegisterEventForPropUI(Prop prop)
    {
        var productName = prop.PropName;
        prop.RegisterButtonClickEvent(() =>
        {
            ManagerSingleton.Instance.PropertyFactory.CreateProduct(productName);
        });
    }
}
