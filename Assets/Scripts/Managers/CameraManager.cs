using Cinemachine;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineBrain cinemachineBrain;

    [Header("Virtual Cameras: "), Space(6)]
    [SerializeField] private CinemachineVirtualCamera setupCamera;
    [SerializeField] private CinemachineVirtualCamera followCarCamera;
    [SerializeField] private bool isSetupCamera;

    public void ChangeToFollowCarCamera()
    {
        setupCamera.gameObject.SetActive(false);
        followCarCamera.gameObject.SetActive(true);
    }

    public void ChangeToSetupCamera()
    {
        setupCamera.gameObject.SetActive(true);
        followCarCamera.gameObject.SetActive(false);
    }

    public void ChangeCamera()
    {
        isSetupCamera = !isSetupCamera;
        if (isSetupCamera) ChangeToSetupCamera();
        else ChangeToFollowCarCamera();
    }
}
