using TMPro;
using UnityEngine;

public enum SetupState
{
    SetupPoints = 0,
    SetupSolidLines = 1,
    SetupJourneyEndpoints = 2,
    DemoCarMove = 3,
}

public class SetUpsController : MonoBehaviour
{
    #region Setup states
    [SerializeField] private SetupState setupState = SetupState.SetupPoints;

    public bool IsSetupPoints() => setupState == SetupState.SetupPoints;
    public bool IsSetupSolidLines() => setupState == SetupState.SetupSolidLines;
    public bool IsSetupJourneyEndpoints() => setupState == SetupState.SetupJourneyEndpoints;
    public bool IsDemoCarMove() => setupState == SetupState.DemoCarMove;

    public SetupState SetupState => setupState;
    public void ChangeSetupState(SetupState setupState)
    {
        UnselectedAllPanels();
        enviroment.PointManager.UnSelecteAllPoints();

        this.setupState = setupState;
        switch (setupState)
        {
            case SetupState.SetupPoints:
                setupPointPanel.OnSelected();
                stateText.text = "Setup points";
                break;

            case SetupState.SetupSolidLines:
                setupSolidLinePanel.OnSelected();
                stateText.text = "Setup solid lines";
                break;

            case SetupState.SetupJourneyEndpoints:
                setupJourneyEndpointsPanel.OnSelected();
                stateText.text = "Setup journey points";
                break;

            case SetupState.DemoCarMove:
                demoCarMove.OnSelected();
                stateText.text = "Demo car move";
                break;

            default: break;
        }
    }

    #endregion

    [SerializeField] private SetupPointsPanel setupPointPanel;
    [SerializeField] private SetupSolidLinePanel setupSolidLinePanel;
    [SerializeField] private SetupJourneyEndpoints setupJourneyEndpointsPanel;
    [SerializeField] private DemoCarMove demoCarMove;

    public SetupPointsPanel SetUpPointPanel => setupPointPanel;
    public SetupSolidLinePanel SetupSolidLinePanel => setupSolidLinePanel;
    public SetupJourneyEndpoints SetupJourneyEndpoints => setupJourneyEndpointsPanel;
    public DemoCarMove DemoCarRun => demoCarMove;

    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private Enviroment enviroment;

    private void Awake() => ChangeSetupState(SetupState.SetupPoints);
    private void UnselectedAllPanels()
    {
        setupPointPanel.OnUnselected();
        setupSolidLinePanel.OnUnselected();
        setupJourneyEndpointsPanel.OnUnselected();
        demoCarMove.OnUnselected();
    }
}
