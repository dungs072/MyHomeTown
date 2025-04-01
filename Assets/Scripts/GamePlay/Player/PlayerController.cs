using UnityEngine;
[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(PlayerWorldSelection))]
public class PlayerController : MonoBehaviour
{
    private CameraController cameraController;
    private PlayerWorldSelection playerWorldSelection;

    void Awake()
    {
        cameraController = GetComponent<CameraController>();
        playerWorldSelection = GetComponent<PlayerWorldSelection>();
    }


    void Update()
    {
        cameraController.UpdateCameraController();
        playerWorldSelection.UpdateSelection();
    }
}
