using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [Header("Buttons"), Space(6)]
    [SerializeField] private Button settingButton;
    [SerializeField] private Button changeCameraButton;

    [Header("Other Components"), Space(6)]
    [SerializeField] private RectTransform clockRect;
    [SerializeField] private TextMeshProUGUI timeText;

    private float timeCount = 0;
    private bool doCount = false;

    public void Update()
    {
        if (!doCount) return;
        timeCount = timeCount + Time.deltaTime;
        CalculateAndShowClock();
    }

    private void CalculateAndShowClock()
    {
        int minutes = Mathf.FloorToInt(timeCount / 60);
        int seconds = Mathf.FloorToInt(timeCount % 60);
        int milliseconds = Mathf.FloorToInt((timeCount * 1000) % 1000);
        timeText.text = $"{minutes:D2}:{seconds:D2}:{milliseconds:D3}";
    }

    public void Awake()
    {
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        changeCameraButton.onClick.AddListener(OnChangeCamera);
        CarManager.EnoughCarArrived += PauseCountTime;
        settingButton.onClick.AddListener(OnSettingInGame);
    }

    public void OnDestroy()
    {
        UnRegisterEvents();
    }

    private void UnRegisterEvents()
    {
        changeCameraButton.onClick.RemoveListener(OnChangeCamera);
        CarManager.EnoughCarArrived -= PauseCountTime;
        settingButton.onClick.RemoveListener(OnSettingInGame);
    }

    public void StartCountTime()
    {
        SetClockStatus(true);
        (timeCount, doCount) = (0, true);
    }

    public void PauseCountTime() => doCount = false;
    public void SetChangeCameraButtonStatus(bool isActive) => changeCameraButton.gameObject.SetActive(isActive);
    public void SetClockStatus(bool isActive) => clockRect.gameObject.SetActive(isActive);

    public void OnChangeCamera() 
    {
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        CameraManager.Instance.ChangeCamera(); 
    }

    private void OnSettingInGame()
    {
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        PopupManager.Instance.OpenPopup<SettingInGamePopup>(PopupManager.PopupType.SettingInGame);
    }
}
