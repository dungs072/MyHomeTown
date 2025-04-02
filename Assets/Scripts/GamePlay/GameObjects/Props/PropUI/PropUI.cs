using UnityEngine;
using UnityEngine.UI;

public class PropUI : CoreBehavior
{
    [SerializeField] private Button rotateButton;
    [SerializeField] private Button moveButton;
    [SerializeField] private Button DestroyButton;

    public override void UpdateBehavior(float dt)
    {
        Vector3 targetPos = Camera.main.transform.position;

        Vector3 direction = targetPos - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        Vector3 euler = lookRotation.eulerAngles;
        transform.rotation = Quaternion.Euler(euler.x, 180, 0);
    }
}
