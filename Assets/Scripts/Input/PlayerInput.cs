using System;
using UnityEngine;
public class PlayerInput : MonoBehaviour
{
    public static event Action<int> OnCameraAngleChanged;

    private InputHandler inputHandler;
    private Vector2 cameraMoveInput;
    private Vector2 zoomInput;
    private int currentAngleCamIndex = 0;

    public Vector2 CameraMoveInput => cameraMoveInput;
    public Vector2 ZoomInput => zoomInput;

    private void Awake()
    {
        inputHandler = new InputHandler();
        RegisterEvents();

    }
    private void RegisterEvents()
    {
        inputHandler.Player.MoveCamera.performed += ctx => cameraMoveInput = ctx.ReadValue<Vector2>();
        inputHandler.Player.MoveCamera.canceled += ctx => cameraMoveInput = Vector2.zero;
        inputHandler.Player.ZoomCamera.performed += ctx => zoomInput = ctx.ReadValue<Vector2>();
        inputHandler.Player.ZoomCamera.canceled += ctx => zoomInput = Vector2.zero;
        inputHandler.Player.RotateCamera.performed += ctx => RotateCamera();


    }

    private void OnEnable() => inputHandler.Enable();
    private void OnDisable() => inputHandler.Disable();

    private void RotateCamera()
    {
        currentAngleCamIndex = (currentAngleCamIndex + 1) % PlayerConfig.CameraZAngles.Length;
        OnCameraAngleChanged?.Invoke(currentAngleCamIndex);
    }
}
