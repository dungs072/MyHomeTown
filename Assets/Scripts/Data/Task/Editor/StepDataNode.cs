

using UnityEngine;
public class StepDataNode
{
    public StepData Data { get; private set; }
    public Rect NodeRect { get; set; }
    public StepDataNode(StepData data, Rect nodeRect)
    {
        Data = data;
        NodeRect = nodeRect;
    }

}