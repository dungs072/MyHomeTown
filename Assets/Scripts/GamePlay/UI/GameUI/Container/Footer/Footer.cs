using UnityEngine;

public class Footer : MonoBehaviour
{
    [SerializeField] private GameObject propHolder;
    [SerializeField] private Prop prop;


    public void CreateProp(string name, Sprite sprite)
    {
        Prop newProp = Instantiate(prop, propHolder.transform);
        newProp.SetName(name);
        newProp.SetBackground(sprite);
    }
}
