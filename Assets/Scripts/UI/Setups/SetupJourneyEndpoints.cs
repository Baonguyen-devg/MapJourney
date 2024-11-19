using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SetupJourneyPointType
{
    None,
    StartPoint,
    EndPoint,
}

public class SetupJourneyEndpoints: SetupPanel
{
    #region Setup Journey states
    public static event Action OnChangedSetStartPoint;
    public static event Action OnChangedSetEndPoint;

    public bool IsSetStartPointMode() => setupJourneyPointType == SetupJourneyPointType.StartPoint;
    public bool IsSetEndPointMode() => setupJourneyPointType == SetupJourneyPointType.EndPoint;

    public SetupJourneyPointType SetupJourneyState => setupJourneyPointType;
    public void ChangeSetupJourneyPointType(SetupJourneyPointType setupJourneyPointType)
    {
        Debug.Log($"[SetupJourneyEndpoints] ChangeSetupJourneyPointType | {setupJourneyPointType}");
        this.setupJourneyPointType = setupJourneyPointType;

        switch (this.setupJourneyPointType)
        {
            case SetupJourneyPointType.StartPoint:
                OnChangedSetStartPoint?.Invoke();
                break;

            case SetupJourneyPointType.EndPoint:
                OnChangedSetEndPoint?.Invoke();
                break;
        }
    }
    #endregion

    [Header("UI Compoents"), Space(6)]
    [SerializeField] private UISwitcher.UISwitcher changeStartPointSwitcher;
    [SerializeField] private UISwitcher.UISwitcher changeEndPointSwitcher;
    [SerializeField] private Button resetAllPointButton;
    [SerializeField] private Button turnOnModeButton;

    [Header("CarInformations"), Space(6)]
    [SerializeField] private TMP_InputField carNumberInput;
    [SerializeField] private CarInformation carInformationPrefab;
    [SerializeField] private RectTransform carInformationHolder;
    [SerializeField] private RectTransform inputs;
    [SerializeField] private List<CarInformation> carInformations = new List<CarInformation>();

    [Header("Other components"), Space(6)]
    [SerializeField] private SetupJourneyPointType setupJourneyPointType = SetupJourneyPointType.StartPoint;
    [SerializeField] private SetUpsController setUpsController;

    public static event Action<List<CarInformation>> CarInformationUpdated;

    private void Awake()
    {
        RegisterEvents();
        ChangeSetupJourneyPointType(SetupJourneyPointType.StartPoint);
        changeStartPointSwitcher.SetWithNotify(true);
        carNumberInput.text = "1";
    }

    private void RegisterEvents()
    {

        turnOnModeButton.onClick.AddListener(OnChangeSetUpMode);
        resetAllPointButton.onClick.AddListener(OnResetAllPoint);

        CarManager.CarDataUpdated += UpdateCarInformations;
        changeStartPointSwitcher.OnValueChanged += OnChangeStartPoint;
        changeEndPointSwitcher.OnValueChanged += OnChangeEndPoint;
    }

    private void OnDestroy()
    {
        UnRegisterEvents();
    }

    private void UnRegisterEvents()
    {
        turnOnModeButton.onClick.RemoveListener(OnChangeSetUpMode);
        resetAllPointButton.onClick.RemoveListener(OnResetAllPoint);

        CarManager.CarDataUpdated -= UpdateCarInformations;
        changeStartPointSwitcher.OnValueChanged -= OnChangeStartPoint;
        changeEndPointSwitcher.OnValueChanged -= OnChangeEndPoint;
    }

    private void OnChangeStartPoint(bool isOn)
    {
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        ChangeSetupJourneyPointType(SetupJourneyPointType.StartPoint);
        changeEndPointSwitcher.SetWithoutNotify(!isOn);
    }

    private void OnChangeEndPoint(bool isOn)
    {
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        ChangeSetupJourneyPointType(SetupJourneyPointType.EndPoint);
        changeStartPointSwitcher.SetWithoutNotify(!isOn);
    }

    private void OnResetAllPoint()
    {
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        Enviroment.Instance.PointManager.ResetAllJourneyPoints();
    }

    private void OnChangeSetUpMode()
    {
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        bool isDemoMode = setUpsController.IsDemoCarMove();
        bool isEnoughCarArrived = Enviroment.Instance.CarManager.IsEnoughCarArrived();
        if (isDemoMode && !isEnoughCarArrived)
        {
            Debug.Log("[SetupJourneyEndpoints] OnChangeSetupMode | In demo mode");
            PopupManager.Instance.OpenAlert(AlertPopup.ALERT_IN_DEMO_MODE);
            return;
        }

        setUpsController.ChangeSetupState(SetupState.SetupJourneyEndpoints);
        Enviroment.Instance.PointManager.ResetAllJourneyPoints();
        Enviroment.Instance.CarManager.DestroyAllCarMovements();
    }

    public void OnCarNumberChange()
    {
        if (int.TryParse(carNumberInput.text, out int carCount))
        {
            Enviroment.Instance.CarManager.SetCarNumber(carCount);
            AdjustCarInformations(carCount);
            UpdateSizeDeltaUI();
        }
    }

    private void SetCarDataDefault()
    {
        CarInformationUpdated?.Invoke(carInformations);
        for (int i = 0; i < carInformations.Count; i++)
        {
            carInformations[i].gameObject.SetActive(true);
            carInformations[i].Init(i, 2, 10);
        }
    }

    private void UpdateCarInformations(List<CarData> carDatas)
    {
        for (int i = 0; i < carInformations.Count; i++)
        {
            carInformations[i].SetAcceleration(carDatas[i].Acceleration);
            carInformations[i].SetInitialSpeed(carDatas[i].InitialSpeed);
            carInformations[i].SetTime(carDatas[i].Time);
        }
    }

    private void UpdateSizeDeltaUI()
    {
        StartCoroutine(UpdateInputRectHeight(carInformationHolder, 5, () =>
        {
            StartCoroutine(UpdateInputRectHeight(inputs, 0, () =>
            {
                StartCoroutine(UpdateInputRectHeight(transform.GetComponent<RectTransform>(), 0, 
                    () => SetCarDataDefault()));
            }));
        }));
    }

    private void AdjustCarInformations(int carCount)
    {
        if (carCount > carInformations.Count)
        {
            int addCarNumber = Mathf.Abs(carInformations.Count - carCount);
            for (int i = 0; i < addCarNumber; i++)
            {
                CarInformation carInformation = Instantiate(carInformationPrefab, carInformationHolder);
                carInformations.Add(carInformation);
            }
        }
        else if (carCount < carInformations.Count)
        {
            int removeCarNumber = carInformations.Count - carCount;
            for (int i = 0; i < removeCarNumber; i++)
            {
                int lastIndex = carInformations.Count - 1;
                Destroy(carInformations[lastIndex].gameObject);
                carInformations.RemoveAt(lastIndex);
            }
        }
    }

    public IEnumerator UpdateInputRectHeight(RectTransform parentRect, float spacing, Action actionCompleted = null)
    {
        yield return new WaitForSeconds(0.2f);
        float totalHeight = 0f;
        for (int i = 0; i < parentRect.childCount; i++)
        {
            RectTransform child = parentRect.GetChild(i).GetComponent<RectTransform>();
            if (child != null)
            {
                totalHeight += child.rect.height;
                totalHeight += spacing;
            }
        }
        totalHeight = Mathf.Max(0, totalHeight);
        parentRect.DOSizeDelta(new Vector2(parentRect.sizeDelta.x, totalHeight), 0.2f).SetEase(Ease.OutCubic);
        actionCompleted?.Invoke();
    }
}