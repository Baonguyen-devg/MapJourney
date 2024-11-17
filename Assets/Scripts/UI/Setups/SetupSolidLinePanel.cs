using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SetUpSolidLinesMode
{
    None,
    AddSolidLineMode,
    RemoveSolidLineMode,
}

public class SetupSolidLinePanel : SetupPanel
{
    #region Setup SolidLine Mode
    public static event Action OnChangedAddMode;
    public static event Action OnChangedRemoveMode;

    public bool IsAddSolidLineState() => setUpSolidsMode == SetUpSolidLinesMode.AddSolidLineMode;
    public bool IsRemoveSolidLineState() => setUpSolidsMode == SetUpSolidLinesMode.RemoveSolidLineMode;

    public SetUpSolidLinesMode SetUpSolidState => setUpSolidsMode;
    public void ChangeSetUpSolidLinesMode(SetUpSolidLinesMode setUpSolidsMode)
    {
        Debug.Log($"[SetupSolidLinePanel] ChangeSetUpSolidLinesMode | {setUpSolidsMode}");
        this.setUpSolidsMode = setUpSolidsMode;

        switch (this.setUpSolidsMode)
        {
            case SetUpSolidLinesMode.AddSolidLineMode:
                OnChangedAddMode?.Invoke();
                break;

            case SetUpSolidLinesMode.RemoveSolidLineMode:
                OnChangedRemoveMode?.Invoke();
                break;
        }
    }
    #endregion

    [Header("UI Compoents"), Space(6)]
    [SerializeField] private UISwitcher.UISwitcher changeAddModeSwitcher;
    [SerializeField] private UISwitcher.UISwitcher changeRemoveModeSwitcher;
    [SerializeField] private Button removeAllSolidLineButton;
    [SerializeField] private Button turnOnModeButton;

    [Header("Other components"), Space(6)]
    [SerializeField] private SetUpSolidLinesMode setUpSolidsMode = SetUpSolidLinesMode.AddSolidLineMode;
    [SerializeField] private SetUpsController setUpsController;

    private void Awake()
    {
        RegisterEvents();
        ChangeSetUpSolidLinesMode(SetUpSolidLinesMode.AddSolidLineMode);
        changeAddModeSwitcher.SetWithNotify(true);
    }

    private void RegisterEvents()
    {
        turnOnModeButton.onClick.AddListener(OnChangeSetUpMode);
        removeAllSolidLineButton.onClick.AddListener(OnRemoveAllSolidLine);

        changeAddModeSwitcher.OnValueChanged += OnChangeAddMode;
        changeRemoveModeSwitcher.OnValueChanged += OnChangeRemoveMode;
    }

    private void OnDestroy()
    {
        UnRegisterEvents();
    }

    private void UnRegisterEvents()
    {
        turnOnModeButton.onClick.AddListener(OnChangeSetUpMode);
        removeAllSolidLineButton.onClick.RemoveListener(OnRemoveAllSolidLine);

        changeAddModeSwitcher.OnValueChanged -= OnChangeAddMode;
        changeRemoveModeSwitcher.OnValueChanged -= OnChangeRemoveMode;
    }

    private void OnChangeAddMode(bool isOn)
    {
        ChangeSetUpSolidLinesMode(SetUpSolidLinesMode.AddSolidLineMode);
        changeRemoveModeSwitcher.SetWithoutNotify(!isOn);
    }

    private void OnChangeRemoveMode(bool isOn)
    {
        ChangeSetUpSolidLinesMode(SetUpSolidLinesMode.RemoveSolidLineMode);
        changeAddModeSwitcher.SetWithoutNotify(!isOn);
    }

    private void OnRemoveAllSolidLine()
    {
        Enviroment.Instance.SolidLineManager.RemoveAllSolidLine();
    }

    private void OnChangeSetUpMode()
    {
        setUpsController.ChangeSetupState(SetupState.SetupSolidLines);
    }
}
