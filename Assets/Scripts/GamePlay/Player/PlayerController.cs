using UnityEngine;
[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(PlayerWorldSelection))]
public class PlayerController : CoreBehavior
{
    private CameraController cameraController;
    private PlayerWorldSelection playerWorldSelection;

    void Awake()
    {
        cameraController = GetComponent<CameraController>();
        playerWorldSelection = GetComponent<PlayerWorldSelection>();
    }

}
