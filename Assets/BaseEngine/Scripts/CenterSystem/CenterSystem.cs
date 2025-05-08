using System.Collections.Generic;
using UnityEngine;

public interface IUpdatable
{
    void UpdateBehavior(float dt);
}

public class CenterSystem : MonoBehaviour
{
    private static CenterSystem _instance;

    public static CenterSystem Instance => _instance;

    private List<CoreBehavior> coreBehaviors = new List<CoreBehavior>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        foreach (var coreBehavior in coreBehaviors)
        {
            if (!coreBehavior.IsExisting) continue;
            coreBehavior.OnStart();
        }
    }
    private void Update()
    {
        var dt = Time.deltaTime;
        for (int i = 0; i < coreBehaviors.Count; i++)
        {
            coreBehaviors[i].UpdateBehavior(dt);
        }
    }

    public void AddCoreBehavior(CoreBehavior coreBehavior)
    {
        if (coreBehaviors.Contains(coreBehavior)) return;
        if (coreBehaviors.Count == 0)
        {
            coreBehaviors.Add(coreBehavior);
            return;
        }
        for (int i = 0; i < coreBehaviors.Count; i++)
        {
            if (coreBehaviors[i].Priority > coreBehavior.Priority)
            {
                coreBehaviors.Insert(i, coreBehavior);
                return;
            }
        }
    }
    public void RemoveCoreBehavior(CoreBehavior coreBehavior)
    {
        if (!coreBehaviors.Contains(coreBehavior)) return;
        coreBehaviors.Remove(coreBehavior);
    }

}
