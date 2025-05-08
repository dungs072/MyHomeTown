using UnityEngine;

public class CoreBehavior : MonoBehaviour, IUpdatable
{
    public virtual int Priority => 1;
    public virtual bool IsExisting => false;
    public virtual void UpdateBehavior(float dt) { }

    void OnEnable()
    {
        OnEnableBehavior();
        HandleEnableBehavior();
        if (IsExisting) return;
        OnStart();
    }

    private void OnEnableBehavior()
    {
        CenterSystem.Instance.AddCoreBehavior(this);
    }
    protected virtual void HandleEnableBehavior()
    {
        //TODO : Add enable behavior
    }
    public virtual void OnStart()
    {
        //TODO : Add start behavior
    }

    void OnDisable()
    {
        OnDisableBehavior();
        HandleDisableBehavior();
    }
    private void OnDisableBehavior()
    {
        CenterSystem.Instance.RemoveCoreBehavior(this);
    }
    protected virtual void HandleDisableBehavior()
    {
        //TODO : Add disable behavior
    }
    void OnDestroy()
    {
        OnDisableBehavior();
        HandleDisableBehavior();
    }

}
