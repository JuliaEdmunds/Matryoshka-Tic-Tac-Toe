using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AddOverlayCamera : MonoBehaviour
{
    [SerializeField] private Camera m_MainCamera;

    private Camera m_OverlayCamera;

    private void Start()
    {
        m_OverlayCamera = SceneController.GetOverlayCamera();

        var cameraData = m_MainCamera.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Add(m_OverlayCamera);
    }
}
