

using UnityEngine;
using UnityEngine.UIElements;
public class StepDataNode
{
    public StepData Data { get; private set; }
    public Vector2 Position { get; set; }
    public StepDataNode(StepData data, Vector2 position)
    {
        Data = data;
        Position = position;
    }

}