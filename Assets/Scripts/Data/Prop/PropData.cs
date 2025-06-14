using UnityEngine;

[CreateAssetMenu(fileName = "PropData", menuName = "Data/Prop/PropData")]
public class PropData : ScriptableObject
{
    [SerializeField] private string propName = "No Name";
    [SerializeField] private string propDescription = "No data available";
    [SerializeField] private int propPrice = 0;
    public string PropName => propName;
    public string PropDescription => propDescription;
    public int PropPrice => propPrice;
}