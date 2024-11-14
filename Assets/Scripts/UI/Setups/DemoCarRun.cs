using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoCarRun : MonoBehaviour
{
    [SerializeField] private Button finishedButton;
    [SerializeField] private SetUpsController setUpsController;
    [SerializeField] private EnviromentController enviromentController;

    private void Awake()
    {
        finishedButton.onClick.AddListener(OnChangeSetupState);
    }

    private void OnEnable() => enviromentController.MoveCar();
    private void OnChangeSetupState() => setUpsController.ChangeNextState();
}
