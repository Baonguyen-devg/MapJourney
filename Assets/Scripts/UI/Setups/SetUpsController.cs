using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SetupState
{
    SetupPoints = 0,
    SetupSolidLines = 1,
    SetupJourneyEndpoints = 2,
    DemoCarMove = 3,
}

public class SetUpsController : Singleton<SetUpsController>
{
    private readonly string SETUP_POINTS = "Setup points";
    private readonly string SETUP_SOLIDLINES = "Setup solid lines";
    private readonly string SETUP_JOURNEY_POINTS = "Setup journey points";
    private readonly string DEMO_CAR_MOVE = "Demo car move";

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
        Enviroment.Instance.PointManager.UnSelecteAllPoints();
        mainUI.SetChangeCameraButtonStatus(false);
        mainUI.SetClockStatus(false);

        this.setupState = setupState;
        switch (setupState)
        {
            case SetupState.SetupPoints:
                setupPointPanel.OnSelected();
                stateText.text = SETUP_POINTS;
                break;

            case SetupState.SetupSolidLines:
                setupSolidLinePanel.OnSelected();
                stateText.text = SETUP_SOLIDLINES;
                break;

            case SetupState.SetupJourneyEndpoints:
                setupJourneyEndpointsPanel.OnSelected();
                stateText.text = SETUP_JOURNEY_POINTS;
                break;

            case SetupState.DemoCarMove:
                mainUI.SetChangeCameraButtonStatus(true);
                StartCoroutine(StartCountTimeCoroutine());
                demoCarMove.OnSelected();
                stateText.text = DEMO_CAR_MOVE;
                break;

            default: break;
        }
    }

    #endregion

    [Header("Panels"), Space(6)]
    [SerializeField] private SetupPointsPanel setupPointPanel;
    [SerializeField] private SetupSolidLinePanel setupSolidLinePanel;
    [SerializeField] private SetupJourneyEndpoints setupJourneyEndpointsPanel;
    [SerializeField] private DemoCarMove demoCarMove;

    public SetupPointsPanel SetUpPointPanel => setupPointPanel;
    public SetupSolidLinePanel SetupSolidLinePanel => setupSolidLinePanel;
    public SetupJourneyEndpoints SetupJourneyEndpoints => setupJourneyEndpointsPanel;
    public DemoCarMove DemoCarRun => demoCarMove;

    [Header("Other Components"), Space(6)]
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private MainUI mainUI;
    [SerializeField] private Button openCloseButton;
    [SerializeField] private bool isOpen = true;

    private void Start()
    {
        ChangeSetupState(SetupState.SetupPoints);
        openCloseButton.onClick.AddListener(OnOpenCloseTool);
    }

    private void OnOpenCloseTool()
    {
        RectTransform rect;
        Vector2 newArchor;
        ChangeStatusAndCalculateNewArchor(out rect, out newArchor);

        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        rect.DOAnchorPos(newArchor, 0.2f).SetEase(Ease.Linear);
    }

    private void ChangeStatusAndCalculateNewArchor(out RectTransform rect, out Vector2 newArchor)
    {
        isOpen = !isOpen;
        rect = gameObject.GetComponent<RectTransform>();
        newArchor = (isOpen) ? new Vector2(rect.sizeDelta.x / 2, rect.anchoredPosition.y)
            : new Vector2(-rect.sizeDelta.x / 2, rect.anchoredPosition.y);
    }

    private void UnselectedAllPanels()
    {
        setupPointPanel.OnUnselected();
        setupSolidLinePanel.OnUnselected();
        setupJourneyEndpointsPanel.OnUnselected();
        demoCarMove.OnUnselected();
    }

    private IEnumerator StartCountTimeCoroutine()
    {
        yield return new WaitForSeconds(1f);
        mainUI.StartCountTime();
    }
}
