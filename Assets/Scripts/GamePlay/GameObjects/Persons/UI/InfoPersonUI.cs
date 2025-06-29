using UnityEngine;
using TMPro;
public class InfoPersonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;

    void Awake()
    {
        CameraDetector.OnCameraMovedOrRotated += FaceUIToCamera;
    }
    void OnDestroy()
    {
        CameraDetector.OnCameraMovedOrRotated -= FaceUIToCamera;
    }
    private void FaceUIToCamera()
    {
        var mainCamera = Camera.main;
        if (mainCamera == null) return;
        transform.LookAt(mainCamera.transform);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
    public void SetInfoText(string text)
    {
        infoText.text = text;
    }
}
