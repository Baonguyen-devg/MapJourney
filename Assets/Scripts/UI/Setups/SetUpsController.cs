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

    public SetupState SetupState => setupState;
    public void ChangeSetupState(SetupState setupState)
    {
        this.setupState = setupState;
        switch (setupState)
        {
            case SetupState.SetupPoints:
                stateText.text = "Setup points";
                break;

            case SetupState.SetupSolidLine: 
                stateText.text = "Setup solid lines";
                break;
            
            case SetupState.SetupRoad: 
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
}
