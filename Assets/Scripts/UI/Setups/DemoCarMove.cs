using UnityEngine;
using UnityEngine.UI;

public class DemoCarMove : SetupPanel
{
    [SerializeField] private Button turnOnModeButton;
    [SerializeField] private SetUpsController setUpsController;

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
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        bool isEnoughJourneyPoint = Enviroment.Instance.PointManager.IsEnoughJourneyPoints();
        if (!isEnoughJourneyPoint)
        {
            Debug.Log("[DemoCarMove] OnChangeSetupMode | Don't enough journey points");
            PopupManager.Instance.OpenAlert(AlertPopup.ALERT_DO_NOT_ENOUGH_JOURNEY_POINT);
            return;
        }

        setUpsController.ChangeSetupState(SetupState.DemoCarMove);
        Enviroment.Instance.CarManager.DemoCarMove();
    }
}
