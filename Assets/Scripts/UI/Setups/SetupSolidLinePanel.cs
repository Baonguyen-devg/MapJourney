using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SetUpSolidLineState
{
    None,
    AddSolidLine,
    RemoveSolidLine,
}

public class SetupSolidLinePanel : MonoBehaviour
{
    #region Setup SolidLine states
    public static event Action OnChangedAddSolidLine;
    public static event Action OnChangedRemoveSolidLine;

    [SerializeField] private SetUpSolidLineState setUpSolidState = SetUpSolidLineState.AddSolidLine;

    public bool IsAddSolidLineState() => setUpSolidState == SetUpSolidLineState.AddSolidLine;
    public bool IsRemoveSolidLineState() => setUpSolidState == SetUpSolidLineState.RemoveSolidLine;

    public SetUpSolidLineState SetUpSolidState => setUpSolidState;
    public void ChangeSetUpSolidLineState(SetUpSolidLineState setUpSolidState)
    {
        Debug.Log($"[SetupSolidLinePanel] ChangeSetUpSolidLineState | {setUpSolidState}");
        this.setUpSolidState = setUpSolidState;

        switch (this.setUpSolidState)
        {
            case SetUpSolidLineState.AddSolidLine:
                OnChangedAddSolidLine?.Invoke();
                stateText.text = "Add";
                break;

            case SetUpSolidLineState.RemoveSolidLine:
                OnChangedRemoveSolidLine?.Invoke();
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
        ChangeSetUpSolidLineState(SetUpSolidLineState.AddSolidLine);
        changeAddStateButton.onClick.AddListener(OnChangeAddState);
        changeRemoveStateButton.onClick.AddListener(OnChangeRemoveState);
        removeAllStateButton.onClick.AddListener(OnRemoveAllSolidLine);
        finishedButton.onClick.AddListener(OnChangeSetupState);
    }

    private void OnChangeAddState() => ChangeSetUpSolidLineState(SetUpSolidLineState.AddSolidLine);
    private void OnChangeRemoveState() => ChangeSetUpSolidLineState(SetUpSolidLineState.RemoveSolidLine);
    private void OnChangeSetupState()
    {
        enviromentController.ResetPoints();
        setUpsController.ChangeNextState();
    }

    private void OnRemoveAllSolidLine()
    {
        enviromentController.RemoveAllSolidLine();
    }
}
