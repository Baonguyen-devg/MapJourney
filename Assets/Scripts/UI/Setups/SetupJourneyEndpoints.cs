using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SetupJourneyState
{
    None,
    SetStartPoint,
    SetEndPoint,
}

public class SetupJourneyEndpoints : MonoBehaviour
{
    #region Setup Journey states
    public static event Action OnChangedSetStartPoint;
    public static event Action OnChangedSetEndPoint;

    [SerializeField] private SetupJourneyState setupJourneyState = SetupJourneyState.SetStartPoint;

    public bool IsSetStartPointState() => setupJourneyState == SetupJourneyState.SetStartPoint;
    public bool IsSetEndPointState() => setupJourneyState == SetupJourneyState.SetEndPoint;

    public SetupJourneyState SetupJourneyState => setupJourneyState;
    public void ChangeSetupJourneyState(SetupJourneyState setupJourneyState)
    {
        Debug.Log($"[SetupJourneyEndpoints] ChangeSetupJourneyState | {setupJourneyState}");
        this.setupJourneyState = setupJourneyState;

        switch (this.setupJourneyState)
        {
            case SetupJourneyState.SetStartPoint:
                OnChangedSetStartPoint?.Invoke();
                stateText.text = "Start Point";
                break;

            case SetupJourneyState.SetEndPoint:
                OnChangedSetEndPoint?.Invoke();
                stateText.text = "End Point";
                break;
        }
    }
    #endregion

    [SerializeField] private Button changeSetStartPointStateButton;
    [SerializeField] private Button changeSetEndPointStateButton;
    [SerializeField] private Button finishedButton;

    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private SetUpsController setUpsController;
    [SerializeField] private EnviromentController enviromentController;

    private void Awake()
    {
        ChangeSetupJourneyState(SetupJourneyState.SetStartPoint);
        changeSetStartPointStateButton.onClick.AddListener(OnChangedSetStartPointState);
        changeSetEndPointStateButton.onClick.AddListener(OnChangedSetEndPointState);
        finishedButton.onClick.AddListener(OnChangeSetupState);
    }

    private void OnChangedSetStartPointState() => ChangeSetupJourneyState(SetupJourneyState.SetStartPoint);
    private void OnChangedSetEndPointState() => ChangeSetupJourneyState(SetupJourneyState.SetEndPoint);
    private void OnChangeSetupState() => setUpsController.ChangeNextState();
}
