using System;
using TMPro;
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

    public bool IsAddPointModeState() => setUpPointsMode == SetUpPointsMode.AddPointMode;
    public bool IsRemovePointModeState() => setUpPointsMode == SetUpPointsMode.RemovePointMode;

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
    [SerializeField] private EnviromentController enviromentController;
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
        ChangeSetUpPointsMode(SetUpPointsMode.AddPointMode);
        changeRemoveModeSwitcher.SetWithoutNotify(!isOn);
    }

    private void OnChangeRemoveMode(bool isOn)
    {
        ChangeSetUpPointsMode(SetUpPointsMode.RemovePointMode);
        changeAddModeSwitcher.SetWithoutNotify(!isOn);
    }

    private void OnRemoveAllPoint()
    {
        enviromentController.RemoveAllPoint();
    }

    private void OnChangeSetUpMode()
    {
        setUpsController.ChangeSetupState(SetupState.SetupPoints);
    }
}
