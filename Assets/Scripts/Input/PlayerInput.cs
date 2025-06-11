using System;
using UnityEngine;
public class PlayerInput : MonoBehaviour
{
    public static event Action<int> OnCameraAngleChanged;
    public static event Action OnSelectionObject;
    public static event Action OnRotateObject;

    private InputHandler inputHandler;
    private Vector2 cameraMoveInput;
    private Vector2 zoomInput;
    private Vector2 lookInput;

    private bool isRotatingCamera = false;
    private int currentAngleCamIndex = 0;

    public Vector2 CameraMoveInput => cameraMoveInput;
    public Vector2 ZoomInput => zoomInput;
    public Vector2 LookInput => lookInput;
    public bool IsRotatingCamera => isRotatingCamera;

    private void Awake()
    {
        inputHandler = new InputHandler();
        RegisterEvents();

    }
    private void RegisterEvents()
    {
        inputHandler.Player.MoveCamera.performed += ctx => cameraMoveInput = ctx.ReadValue<Vector2>();
        inputHandler.Player.MoveCamera.canceled += ctx => cameraMoveInput = Vector2.zero;
        inputHandler.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputHandler.Player.Look.canceled += ctx => lookInput = Vector2.zero;
        inputHandler.Player.ZoomCamera.performed += ctx => zoomInput = ctx.ReadValue<Vector2>();
        inputHandler.Player.ZoomCamera.canceled += ctx => zoomInput = Vector2.zero;
        inputHandler.Player.RotateCamera.performed += ctx => isRotatingCamera = true;
        inputHandler.Player.RotateCamera.canceled += ctx => isRotatingCamera = false;

        inputHandler.Player.SelectionObject.performed += ctx => SelectObject();
        // select only
        // inputHandler.Player.SelectionObject.canceled += ctx => SelectObject();

        inputHandler.Player.RotateObject.performed += ctx => RotateObject();
        inputHandler.Player.RotateObject.canceled -= ctx => RotateObject();

    }

    private void OnEnable() => inputHandler.Enable();
    private void OnDisable() => inputHandler.Disable();

    private void RotateCamera()
    {
        currentAngleCamIndex = (currentAngleCamIndex + 1) % CameraConfig.CameraZAngles.Length;
        OnCameraAngleChanged?.Invoke(currentAngleCamIndex);
    }
    private void SelectObject()
    {
        OnSelectionObject?.Invoke();
    }

    private void RotateObject()
    {
        OnRotateObject?.Invoke();
    }
}
