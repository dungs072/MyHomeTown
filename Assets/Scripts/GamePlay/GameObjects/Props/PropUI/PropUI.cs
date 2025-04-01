using UnityEngine;
using UnityEngine.UI;

public class PropUI : MonoBehaviour
{
    [SerializeField] private Button rotateButton;
    [SerializeField] private Button moveButton;
    [SerializeField] private Button DestroyButton;

    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0, 180, 0);
    }
}
