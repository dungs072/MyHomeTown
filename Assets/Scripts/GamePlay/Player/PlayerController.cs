using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private Camera playerCamera;
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerCamera = Camera.main;
    }

    void OnEnable()
    {
        PlayerInput.OnCameraAngleChanged += OnCameraAngleChanged;
    }
    void OnDisable()
    {
        PlayerInput.OnCameraAngleChanged -= OnCameraAngleChanged;
    }


    void Update()
    {
        UpdateCameraMovement();
        UpdateCameraZoom();
    }

    private void UpdateCameraMovement()
    {
        Vector2 CameraMoveInput = playerInput.CameraMoveInput;
        Vector3 moveDelta = new Vector3(CameraMoveInput.x, 0, CameraMoveInput.y) * PlayerConfig.MOVEMENT_SPEED * Time.deltaTime;
        playerCamera.transform.position += moveDelta;
    }
    private void UpdateCameraZoom()
    {
        Vector2 zoomInput = playerInput.ZoomInput;
        float zoomDelta = zoomInput.y * PlayerConfig.ZOOM_SPEED * Time.deltaTime;
        playerCamera.transform.position += playerCamera.transform.forward * zoomDelta;
    }

    private void OnCameraAngleChanged(int angleIndex)
    {
        var camQuaternion = playerCamera.transform.rotation;
        Vector3 eulerRotation = camQuaternion.eulerAngles;

        playerCamera.transform.rotation = Quaternion.Euler(eulerRotation.x, PlayerConfig.CameraZAngles[angleIndex], eulerRotation.z);
    }

}
