using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private Button settingButton;
    [SerializeField] private Button changeCameraButton;
    [SerializeField] private RectTransform clockRect;
    [SerializeField] private TextMeshProUGUI timeText;

    private float timeCount = 0;
    private bool doCount = false;

    public void Update()
    {
        if (!doCount) return;
        timeCount = timeCount + Time.deltaTime;

        int minutes = Mathf.FloorToInt(timeCount / 60);
        int seconds = Mathf.FloorToInt(timeCount % 60);
        int milliseconds = Mathf.FloorToInt((timeCount * 1000) % 1000);
        timeText.text = $"{minutes:D2}:{seconds:D2}:{milliseconds:D3}";
    }

    public void Awake()
    {
        changeCameraButton.onClick.AddListener(OnChangeCamera);
        CarManager.EnoughCarArrived += PauseCountTime;
    }

    public void OnDestroy()
    {
        changeCameraButton.onClick.RemoveListener(OnChangeCamera);
        CarManager.EnoughCarArrived -= PauseCountTime;
    }

    public void StartCountTime()
    {
        SetClockStatus(true);
        (timeCount, doCount) = (0, true);
    }

    public void PauseCountTime() => doCount = false;
    public void OnChangeCamera() => CameraManager.Instance.ChangeCamera();
    public void SetChangeCameraButtonStatus(bool isActive) => changeCameraButton.gameObject.SetActive(isActive);
    public void SetClockStatus(bool isActive) => clockRect.gameObject.SetActive(isActive);
}
