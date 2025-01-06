using System;
using UnityEngine;

public class ActionHolder : MonoBehaviour
{
    [SerializeField] private ActionData actionData;
    [SerializeField] private bool isBusy = false;
    public ActionData ActionData => actionData;

    public bool IsBusy => isBusy;
    public void SetBusy(bool isBusy)
    {
        this.isBusy = isBusy;
    }

    private void OnDrawGizmos()
    {
        if (this.isBusy)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 5f);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 5f);
        }
    }


}
