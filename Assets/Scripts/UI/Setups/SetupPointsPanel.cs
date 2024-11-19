using System;
using UnityEngine;
using UnityEngine.UI;

public enum SetUpPointsMode
{
    None,
    AddPointMode,
    RemovePointMode,
}

public class SetupPointsPanel : SetupPanel
{
    #region Setup points mode
    public static event Action OnChangedAddMode;
    public static event Action OnChangedRemoveMode;

    public bool IsAddPointMode() => setUpPointsMode == SetUpPointsMode.AddPointMode;
    public bool IsRemovePointMode() => setUpPointsMode == SetUpPointsMode.RemovePointMode;

    public SetUpPointsMode SetUpPointState => setUpPointsMode;
    public void ChangeSetUpPointsMode(SetUpPointsMode setUpPointsMode)
    {
        Debug.Log($"[SetupPointPanel] ChangeSetUpPointsMode | {setUpPointsMode}");
        this.setUpPointsMode = setUpPointsMode;

        switch (this.setUpPointsMode)
        {
            case SetUpPointsMode.AddPointMode:
                OnChangedAddMode?.Invoke();
                break;

            case SetUpPointsMode.RemovePointMode:
                OnChangedRemoveMode?.Invoke();
                break;
        }
    }
    #endregion

    [Header("UI Compoents"), Space(6)]
    [SerializeField] private UISwitcher.UISwitcher changeAddModeSwitcher;
    [SerializeField] private UISwitcher.UISwitcher changeRemoveModeSwitcher;
    [SerializeField] private Button removeAllPointButton;
    [SerializeField] private Button turnOnModeButton;

    [Header("Other components"), Space(6)]
    [SerializeField] private SetUpPointsMode setUpPointsMode = SetUpPointsMode.AddPointMode;
    [SerializeField] private SetUpsController setUpsController;

    private void Awake()
    {
        RegisterEvents();
        ChangeSetUpPointsMode(SetUpPointsMode.AddPointMode);
        changeAddModeSwitcher.SetWithNotify(true);
    }

    private void RegisterEvents()
    {
        turnOnModeButton.onClick.AddListener(OnChangeSetUpMode);
        removeAllPointButton.onClick.AddListener(OnRemoveAllPoint);

        changeAddModeSwitcher.OnValueChanged += OnChangeAddMode;
        changeRemoveModeSwitcher.OnValueChanged += OnChangeRemoveMode;
    }

    private void OnDestroy()
    {
        UnRegisterEvents();
    }

    private void UnRegisterEvents()
    {
        turnOnModeButton.onClick.RemoveListener(OnChangeSetUpMode);
        removeAllPointButton.onClick.RemoveListener(OnRemoveAllPoint);
        
        changeAddModeSwitcher.OnValueChanged -= OnChangeAddMode;
        changeRemoveModeSwitcher.OnValueChanged -= OnChangeRemoveMode;
    }

    private void OnChangeAddMode(bool isOn)
    {
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        ChangeSetUpPointsMode(SetUpPointsMode.AddPointMode);
        changeRemoveModeSwitcher.SetWithoutNotify(!isOn);
    }

    private void OnChangeRemoveMode(bool isOn)
    {
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        ChangeSetUpPointsMode(SetUpPointsMode.RemovePointMode);
        changeAddModeSwitcher.SetWithoutNotify(!isOn);
    }

    private void OnRemoveAllPoint()
    {
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        Enviroment.Instance.PointManager.RemoveAllPoint();
    }

    private void OnChangeSetUpMode()
    {
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        bool isDemoMode = setUpsController.IsDemoCarMove();
        bool isEnoughCarArrived = Enviroment.Instance.CarManager.IsEnoughCarArrived();
        if (isDemoMode && !isEnoughCarArrived)
        {
            Debug.Log("[SetupPointsPanel] OnChangeSetupMode | In demo mode");
            PopupManager.Instance.OpenAlert(AlertPopup.ALERT_IN_DEMO_MODE);
            return;
        }

        setUpsController.ChangeSetupState(SetupState.SetupPoints);
    }
}
