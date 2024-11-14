using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public enum SetupState
{
    SetupPoints = 0,
    SetupSolidLine = 1,
    SetupJourneyEndpoints = 2,
    DemoCarMove = 3,
    SetupRoad = 4,
    SetupCarPosition = 5,
}

public class SetUpsController : MonoBehaviour
{
    #region Setup states
    [SerializeField] private SetupState setupState = SetupState.SetupPoints;

    public void ChangeNextState()
    {
        int nextIndex = (int)setupState + 1;
        if (nextIndex >= System.Enum.GetValues(typeof(SetupState)).Length)
        {
            nextIndex = 0;
        }
        ChangeSetupState((SetupState)nextIndex);
    }

    public bool IsSetupPoint() => setupState == SetupState.SetupPoints;
    public bool IsSetupSolidLine() => setupState == SetupState.SetupSolidLine;
    public bool IsSetupJourneyEndpoints() => setupState == SetupState.SetupJourneyEndpoints;
    public bool IsDemoCarMove() => setupState == SetupState.DemoCarMove;
    public bool IsSetupRoad() => setupState == SetupState.SetupRoad;
    public bool IsSetupCarPosition() => setupState == SetupState.SetupCarPosition;

    public SetupState SetupState => setupState;
    public void ChangeSetupState(SetupState setupState)
    {
        DisactiveAllPanel();
        this.setupState = setupState;
        switch (setupState)
        {
            case SetupState.SetupPoints:
                setupPointPanel.gameObject.SetActive(true);
                stateText.text = "Setup points";
                break;

            case SetupState.SetupSolidLine:
                setupSolidLinePanel.gameObject.SetActive(true);
                stateText.text = "Setup solid lines";
                break;

            case SetupState.SetupJourneyEndpoints:
                setupJourneyEndpointsPanel.gameObject.SetActive(true);
                stateText.text = "Setup journey end points";
                break;

            case SetupState.DemoCarMove:
                demoCarRun.gameObject.SetActive(true);
                stateText.text = "Demo car move";
                break;

            case SetupState.SetupRoad:
                setupRoadPanel.gameObject.SetActive(true);
                stateText.text = "Setup roads";
                break;

            case SetupState.SetupCarPosition:
                stateText.text = "Setup car position";
                break;

            default: break;
        }
    }

    #endregion

    [SerializeField] private SetupPointPanel setupPointPanel;
    [SerializeField] private SetupSolidLinePanel setupSolidLinePanel;
    [SerializeField] private SetupJourneyEndpoints setupJourneyEndpointsPanel;
    [SerializeField] private DemoCarRun demoCarRun;
    [SerializeField] private SetupRoadPanel setupRoadPanel;

    public SetupPointPanel SetUpPointPanel => setupPointPanel;
    public SetupSolidLinePanel SetupSolidLinePanel => setupSolidLinePanel;
    public SetupJourneyEndpoints SetupJourneyEndpoints => setupJourneyEndpointsPanel;
    public DemoCarRun DemoCarRun => demoCarRun;
    public SetupRoadPanel SetupRoadPanel => setupRoadPanel;

    [SerializeField] private TextMeshProUGUI stateText;

    private void Awake()
    {
        ChangeSetupState(SetupState.SetupPoints);
    }

    private void DisactiveAllPanel()
    {
        setupPointPanel.gameObject.SetActive(false);
        setupSolidLinePanel.gameObject.SetActive(false);
        setupJourneyEndpointsPanel.gameObject.SetActive(false);
        setupRoadPanel.gameObject.SetActive(false);
        demoCarRun.gameObject.SetActive(false);
    }
}
