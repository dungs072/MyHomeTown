using NUnit.Framework;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private PlayerInput playerInput;
    private Camera playerCamera;

    private Vector2 movementInput;

    private float zoomInput;

    private float movementSpeed;
    private MapWorld mapWorld;
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerCamera = Camera.main;
        movementInput = Vector2.zero;
        zoomInput = 0;
        movementSpeed = CameraConfig.MIN_MOVEMENT_SPEED;
    }
    void OnEnable()
    {
        PlayerInput.OnCameraAngleChanged += OnCameraAngleChanged;
    }
    void OnDisable()
    {

        PlayerInput.OnCameraAngleChanged -= OnCameraAngleChanged;
    }

    void Start()
    {
        mapWorld = ManagerSingleton.Instance.MapWorld;
    }

    void Update()
    {
        movementSpeed = GetMovementSpeed();
        UpdateInputOverTime();
        UpdateCameraMovements();
        UpdateCameraZoom();
        UpdateCameraRotation();
    }

    private float GetMovementSpeed()
    {
        //! start height will start at 0
        float currentHeight = playerCamera.transform.position.y;
        float heightPercent = (currentHeight - CameraConfig.MIN_CAMERA_HEIGHT) / (CameraConfig.MAX_CAMERA_HEIGHT - CameraConfig.MIN_CAMERA_HEIGHT);
        float movementSpeed = heightPercent * (CameraConfig.MAX_MOVEMENT_SPEED + CameraConfig.MIN_MOVEMENT_SPEED) + CameraConfig.MIN_MOVEMENT_SPEED;
        return movementSpeed;
    }

    private void UpdateInputOverTime()
    {
        UpdateMovementInput();
        UpdateZoomInput();
    }
    private void UpdateMovementInput()
    {
        movementInput += playerInput.CameraMoveInput;
        if (movementInput.magnitude > 0.01f)
        {
            movementInput *= Mathf.Exp(CameraConfig.MOVEMENT_ACCELERATION * Time.deltaTime);
        }
        else
        {
            movementInput = Vector2.zero;
        }
    }

    private void UpdateZoomInput()
    {
        zoomInput += playerInput.ZoomInput.y;
        if (Mathf.Abs(zoomInput) > 0.01f)
        {
            zoomInput *= Mathf.Exp(CameraConfig.ZOOM_ACCELERATION * Time.deltaTime);
        }
        else
        {
            zoomInput = 0;
        }
    }
    private void UpdateCameraMovements()
    {
        UpdateCameraMovement();
        UpdateCameraMovementWithMouse();
    }

    private void UpdateCameraMovement()
    {


        Vector2 CameraMoveInput = movementInput;
        Vector3 forward = playerCamera.transform.up;
        forward.y = 0;
        Vector3 moveDirection = forward * CameraMoveInput.y +
                                 playerCamera.transform.right * CameraMoveInput.x;

        var position = playerCamera.transform.position + moveDirection * movementSpeed * Time.deltaTime;
        UpdateCameraPosition(position);
    }



    private void UpdateCameraMovementWithMouse()
    {
        if (playerInput.IsRotatingCamera) return;
        Vector3 moveDirection = Vector3.zero;

        // Get screen width and height
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        Vector3 mousePos = Input.mousePosition;

        // Check if the mouse is near the screen edges
        if (mousePos.x <= CameraConfig.BORDER_THICKNESS)  // Left
        {
            moveDirection.x = -CameraConfig.CAMERA_MOUSE_BORDER_MOVEMENT;
        }
        else if (mousePos.x >= screenWidth - CameraConfig.BORDER_THICKNESS)  // Right
        {
            moveDirection.x = CameraConfig.CAMERA_MOUSE_BORDER_MOVEMENT;
        }

        if (mousePos.y <= CameraConfig.BORDER_THICKNESS)  // Bottom
        {
            moveDirection.z = -CameraConfig.CAMERA_MOUSE_BORDER_MOVEMENT;
        }
        else if (mousePos.y >= screenHeight - CameraConfig.BORDER_THICKNESS)  // Top
        {
            moveDirection.z = CameraConfig.CAMERA_MOUSE_BORDER_MOVEMENT;
        }

        // Move the camera
        Vector3 forward = playerCamera.transform.up;
        Vector3 right = playerCamera.transform.right;
        forward.y = 0;
        moveDirection = forward * moveDirection.z +
                        right * moveDirection.x;
        playerCamera.transform.position += moveDirection * movementSpeed * Time.deltaTime;
    }
    private void UpdateCameraZoom()
    {
        if (!IsAboveGround() && zoomInput > 0)
        {
            return;
        }
        if (!IsBelowSky() && zoomInput < 0)
        {
            return;
        }


        float zoomDelta = zoomInput * CameraConfig.ZOOM_SPEED * Time.deltaTime;

        var position = playerCamera.transform.position + playerCamera.transform.forward * zoomDelta;
        UpdateCameraPosition(position);
    }

    private void UpdateCameraPosition(Vector3 position)
    {
        if (position.x < mapWorld.LeftBound || position.x > mapWorld.RightBound)
        {
            position.x = Mathf.Clamp(position.x, mapWorld.LeftBound, mapWorld.RightBound);
        }
        if (position.z < mapWorld.BottomBound || position.z > mapWorld.TopBound)
        {
            position.z = Mathf.Clamp(position.z, mapWorld.BottomBound, mapWorld.TopBound);
        }
        playerCamera.transform.position = position;
    }

    private void UpdateCameraRotation()
    {
        if (!playerInput.IsRotatingCamera) return;
        float mouseX = playerInput.LookInput.x;
        float angleY = playerCamera.transform.eulerAngles.y + mouseX * CameraConfig.ROTATION_SPEED * Time.deltaTime;
        playerCamera.transform.rotation = Quaternion.Euler(
            playerCamera.transform.eulerAngles.x,
            angleY,
            playerCamera.transform.eulerAngles.z
        );
    }

    private void OnCameraAngleChanged(int angleIndex)
    {
        var camQuaternion = playerCamera.transform.rotation;
        Vector3 eulerRotation = camQuaternion.eulerAngles;

        playerCamera.transform.rotation = Quaternion.Euler(eulerRotation.x, CameraConfig.CameraZAngles[angleIndex], eulerRotation.z);
    }


    bool IsAboveGround()
    {
        if (zoomInput == 0) return true;
        Ray ray = new Ray(playerCamera.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.distance > CameraConfig.MIN_CAMERA_HEIGHT;
        }
        return playerCamera.transform.position.y - CameraConfig.MIN_CAMERA_HEIGHT > 0;
    }
    bool IsBelowSky()
    {
        return playerCamera.transform.position.y < CameraConfig.MAX_CAMERA_HEIGHT;
    }
}
