using UnityEngine;

public class ActionHolder : MonoBehaviour
{
    [SerializeField] private ActionData actionData;
    [SerializeField] private bool isBusy = false;

    public bool IsBusy => isBusy;
    public void SetBusy(bool isBusy)
    {
        this.isBusy = isBusy;
    }

    public ActionData ActionData => actionData;

}
