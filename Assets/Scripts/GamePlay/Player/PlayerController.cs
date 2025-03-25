using UnityEngine;
[RequireComponent(typeof(CameraController))]
public class PlayerController : MonoBehaviour
{
    private CameraController cameraController;


    void Awake()
    {
        cameraController = GetComponent<CameraController>();
    }


    void Update()
    {
        cameraController.UpdateCameraController();
    }
}
