using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [Header("Main Camera"), Space(6)]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineBrain cinemachineBrain;

    [Header("Virtual Cameras"), Space(6)]
    [SerializeField] private CinemachineVirtualCamera setupCamera;
    [SerializeField] private CinemachineVirtualCamera followCarCameraPrefab;

    [SerializeField] private Transform virtualCameraHolder;
    [SerializeField] private List<CinemachineVirtualCamera> virtualCameras = new List<CinemachineVirtualCamera>();
    [SerializeField] private bool isSetupCamera;
    [SerializeField] private int cameraPresentIndex = 0;

    public void CreateVirtualCameraFollowCar(List<CarMovement> carMovements)
    {
        CleanAllVirualCameras();
        CreateVirtualCameraFollowCar(carMovements, setupCamera.Priority);
        DisplaySetupVirtualCamera();
    }

    private void CreateVirtualCameraFollowCar(List<CarMovement> carMovements, int priority)
    {
        virtualCameras.Add(setupCamera);
        foreach (CarMovement carMoving in carMovements)
        {
            priority = priority + 1;
            CreateVirtualCameraFollowCar(carMoving, priority);
        }
    }

    private void CleanAllVirualCameras()
    {
        if (virtualCameras.Count != 0)
        {
            for (int i = virtualCameras.Count - 1; i >= 1; i--)
            {
                Destroy(virtualCameras[i].gameObject);
            }
        }

        virtualCameras.Clear();
    }

    private void DisplaySetupVirtualCamera()
    {
        cameraPresentIndex = 0;
        virtualCameras[cameraPresentIndex].gameObject.SetActive(true);
    }

    public void CreateVirtualCameraFollowCar(CarMovement carMovement, int priority)
    {
        var virtualCamera = Instantiate(followCarCameraPrefab, virtualCameraHolder);
        virtualCamera.Follow = carMovement.transform;
        virtualCamera.LookAt = carMovement.transform;
        virtualCamera.Priority = priority;
        virtualCameras.Add(virtualCamera);
    }

    public void ChangeCamera()
    {
        virtualCameras[cameraPresentIndex].gameObject.SetActive(false);
        cameraPresentIndex = cameraPresentIndex + 1;
        if (cameraPresentIndex >= virtualCameras.Count) cameraPresentIndex = 0;
        virtualCameras[cameraPresentIndex].gameObject.SetActive(true);
    }
}
