using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private Button settingButton;
    [SerializeField] private Button changeCameraButton;

    public void Awake()
    {
        changeCameraButton.onClick.AddListener(OnChangeCamera);
    }

    public void OnChangeCamera() => CameraManager.Instance.ChangeCamera();
    public void SetChangeCameraButtonStatus(bool isActive) => changeCameraButton.gameObject.SetActive(isActive);
}
