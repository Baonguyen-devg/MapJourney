using System;
using UnityEngine;
using UnityEngine.UI;

public enum SetupJourneyPointType
{
    None,
    StartPoint,
    EndPoint,
}

public class SetupJourneyEndpoints: SetupPanel
{
    #region Setup Journey states
    public static event Action OnChangedSetStartPoint;
    public static event Action OnChangedSetEndPoint;

    public bool IsSetStartPointMode() => setupJourneyPointType == SetupJourneyPointType.StartPoint;
    public bool IsSetEndPointMode() => setupJourneyPointType == SetupJourneyPointType.EndPoint;

    public SetupJourneyPointType SetupJourneyState => setupJourneyPointType;
    public void ChangeSetupJourneyPointType(SetupJourneyPointType setupJourneyPointType)
    {
        Debug.Log($"[SetupJourneyEndpoints] ChangeSetupJourneyPointType | {setupJourneyPointType}");
        this.setupJourneyPointType = setupJourneyPointType;

        switch (this.setupJourneyPointType)
        {
            case SetupJourneyPointType.StartPoint:
                OnChangedSetStartPoint?.Invoke();
                break;

            case SetupJourneyPointType.EndPoint:
                OnChangedSetEndPoint?.Invoke();
                break;
        }
    }
    #endregion

    [Header("UI Compoents"), Space(6)]
    [SerializeField] private UISwitcher.UISwitcher changeStartPointSwitcher;
    [SerializeField] private UISwitcher.UISwitcher changeEndPointSwitcher;
    [SerializeField] private Button resetAllPointButton;
    [SerializeField] private Button turnOnModeButton;

    [Header("Other components"), Space(6)]
    [SerializeField] private SetupJourneyPointType setupJourneyPointType = SetupJourneyPointType.StartPoint;
    [SerializeField] private Enviroment enviromentController;
    [SerializeField] private SetUpsController setUpsController;

    private void Awake()
    {
        RegisterEvents();
        ChangeSetupJourneyPointType(SetupJourneyPointType.StartPoint);
        changeStartPointSwitcher.SetWithNotify(true);
    }

    private void RegisterEvents()
    {
        turnOnModeButton.onClick.AddListener(OnChangeSetUpMode);
        resetAllPointButton.onClick.AddListener(OnResetAllPoint);

        changeStartPointSwitcher.OnValueChanged += OnChangeStartPoint;
        changeEndPointSwitcher.OnValueChanged += OnChangeEndPoint;
    }

    private void OnDestroy()
    {
        UnRegisterEvents();
    }

    private void UnRegisterEvents()
    {
        turnOnModeButton.onClick.RemoveListener(OnChangeSetUpMode);
        resetAllPointButton.onClick.RemoveListener(OnResetAllPoint);

        changeStartPointSwitcher.OnValueChanged -= OnChangeStartPoint;
        changeEndPointSwitcher.OnValueChanged -= OnChangeEndPoint;
    }

    private void OnChangeStartPoint(bool isOn)
    {
        ChangeSetupJourneyPointType(SetupJourneyPointType.StartPoint);
        changeEndPointSwitcher.SetWithoutNotify(!isOn);
    }

    private void OnChangeEndPoint(bool isOn)
    {
        ChangeSetupJourneyPointType(SetupJourneyPointType.EndPoint);
        changeStartPointSwitcher.SetWithoutNotify(!isOn);
    }

    private void OnResetAllPoint()
    {
        enviromentController.PointManager.ResetAllJourneyPoint();
    }

    private void OnChangeSetUpMode()
    {
        setUpsController.ChangeSetupState(SetupState.SetupJourneyEndpoints);
        enviromentController.PointManager.ResetAllJourneyPoint();
    }
}