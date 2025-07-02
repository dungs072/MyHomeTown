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
    private Quaternion lastRotation;
    void LateUpdate()
    {
        if (lastPosition != transform.position)
        {
            OnCameraMovedOrRotated?.Invoke();
            lastPosition = transform.position;
        }
        if (lastRotation != transform.rotation)
        {
            OnCameraMovedOrRotated?.Invoke();
            lastRotation = transform.rotation;
        }
    }
}
