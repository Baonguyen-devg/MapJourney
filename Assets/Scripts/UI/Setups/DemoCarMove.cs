using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoCarMove : SetupPanel
{
    [SerializeField] private Button turnOnModeButton;
    [SerializeField] private SetUpsController setUpsController;
    [SerializeField] private Enviroment enviromentController;

    private void Awake()
    {
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        turnOnModeButton.onClick.AddListener(OnChangeSetUpMode);
    }

    private void OnDestroy()
    {
        UnRegisterEvents();
    }

    private void UnRegisterEvents()
    {
        turnOnModeButton.onClick.RemoveListener(OnChangeSetUpMode);
    }

    private void OnChangeSetUpMode()
    {
        setUpsController.ChangeSetupState(SetupState.DemoCarMove);
        enviromentController.DemoCarMove();
    }
}
