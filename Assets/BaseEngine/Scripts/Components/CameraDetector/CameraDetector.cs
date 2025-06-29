using UnityEngine;
using System;
/// <summary>
/// This script is used to detect movement of the camera.
/// It can be used to trigger events or actions based on camera movement.
/// </summary>
public class CameraDetector : MonoBehaviour
{
    public static event Action OnCameraMovedOrRotated;
    private Vector3 lastPosition;
    void LateUpdate()
    {
        lastPosition = transform.position;
        if (lastPosition == transform.position) return;
        OnCameraMovedOrRotated?.Invoke();
    }
}
