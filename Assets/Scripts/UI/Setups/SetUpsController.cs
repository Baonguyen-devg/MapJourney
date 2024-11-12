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
    SetupRoad = 2,
    SetupCarPosition = 3,
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
    [SerializeField] private SetupRoadPanel setupRoadPanel;

    public SetupPointPanel SetUpPointPanel => setupPointPanel;
    public SetupSolidLinePanel SetupSolidLinePanel => setupSolidLinePanel;
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
        setupRoadPanel.gameObject.SetActive(false);
    }
}
