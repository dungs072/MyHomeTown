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
    void Start()
    {
        FaceUIToCamera();
    }
    private void FaceUIToCamera()
    {
        var mainCamera = Camera.main;
        if (mainCamera == null) return;
        if (!Utils.IsInView(mainCamera, transform)) return;
        Vector3 direction = transform.position - mainCamera.transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }
    public void SetInfoText(string text)
    {
        infoText.text = text;
    }
}
