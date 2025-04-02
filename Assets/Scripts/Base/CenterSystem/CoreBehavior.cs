using UnityEngine;

public class CoreBehavior : MonoBehaviour, IUpdatable
{
    public virtual void UpdateBehavior(float dt) { }

    void OnEnable()
    {
        OnEnableBehavior();
    }

    protected virtual void OnEnableBehavior()
    {
        CenterSystem.Instance.AddCoreBehavior(this);
    }

    void OnDisable()
    {
        OnDisableBehavior();
    }
    protected virtual void OnDisableBehavior()
    {
        CenterSystem.Instance.RemoveCoreBehavior(this);
    }
    void OnDestroy()
    {
        OnDisableBehavior();
    }

}
