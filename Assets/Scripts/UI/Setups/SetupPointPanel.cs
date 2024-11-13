using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SetUpPointState
{
    None,
    AddPoint,
    RemovePoint,
}

public class SetupPointPanel : MonoBehaviour
{
    #region Setup point states
    public static event Action OnChangedAddPoint;
    public static event Action OnChangedRemovePoint;

    [SerializeField] private SetUpPointState setUpPointState = SetUpPointState.AddPoint;

    public bool IsAddPointState() => setUpPointState == SetUpPointState.AddPoint;
    public bool IsRemovePointState() => setUpPointState == SetUpPointState.RemovePoint;

    public SetUpPointState SetUpPointState => setUpPointState;
    public void ChangeSetUpPointState(SetUpPointState setUpPointState)
    {
        Debug.Log($"[SetupPointPanel] ChangeSetUpPointState | {setUpPointState}");
        this.setUpPointState = setUpPointState;

        switch (this.setUpPointState)
        {
            case SetUpPointState.AddPoint:
                OnChangedAddPoint?.Invoke();
                stateText.text = "Add";
                break;

            case SetUpPointState.RemovePoint:
                OnChangedRemovePoint?.Invoke();
                stateText.text = "Remove";
                break;
        }
    }
    #endregion

    [SerializeField] private Button changeAddStateButton;
    [SerializeField] private Button changeRemoveStateButton;
    [SerializeField] private Button removeAllStateButton;
    [SerializeField] private Button finishedButton;

    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private SetUpsController setUpsController;
    [SerializeField] private EnviromentController enviromentController;

    private void Awake()
    {
        ChangeSetUpPointState(SetUpPointState.AddPoint);
        changeAddStateButton.onClick.AddListener(OnChangeAddState);
        changeRemoveStateButton.onClick.AddListener(OnChangeRemoveState);
        removeAllStateButton.onClick.AddListener(OnRemoveAllPoint);
        finishedButton.onClick.AddListener(OnChangeSetupState);
    }

    private void OnChangeAddState() => ChangeSetUpPointState(SetUpPointState.AddPoint);
    private void OnChangeRemoveState() => ChangeSetUpPointState(SetUpPointState.RemovePoint);
    private void OnChangeSetupState() => setUpsController.ChangeNextState();

    private void OnRemoveAllPoint()
    {
        enviromentController.RemoveAllPoint();
    }
}
