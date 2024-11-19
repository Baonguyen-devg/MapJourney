using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Other components"), Space(6)]
    [SerializeField] private RectTransform logoRect;

    [Header("Buttons"), Space(6)]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        RegisterEvents();
        ActiveLogoEffect();
    }

    private void ActiveLogoEffect()
    {
        logoRect.DOScale(1.01f, 1f).SetEase(Ease.InOutSine)
           .SetLoops(-1, LoopType.Yoyo);
    }

    private void RegisterEvents()
    {
        playButton.onClick.AddListener(OnPlay);
        settingButton.onClick.AddListener(OnDisplaySetting);
        creditButton.onClick.AddListener(OnDisplayCredit);
        exitButton.onClick.AddListener(OnExitGame);
    }

    private void OnDestroy()
    {
        UnRegisterEvents();
    }

    private void UnRegisterEvents()
    {
        playButton.onClick.RemoveListener(OnPlay);
        settingButton.onClick.RemoveListener(OnDisplaySetting);
        creditButton.onClick.RemoveListener(OnDisplayCredit);
        exitButton.onClick.RemoveListener(OnExitGame);
    }

    private void OnPlay()
    {
        Debug.Log("[MainMenuUI] OnPlay | Load Scene Gameplay");
        Enviroment.Instance.ResetGame();
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.GameplayBackground);
        SceneManager.LoadScene("GamePlay");
    }

    private void OnDisplayCredit()
    {
        Debug.Log("[MainMenuUI] OnDisplayCredit | Display credit");
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        PopupManager.Instance.OpenPopup<CreditPopup>(PopupManager.PopupType.Credit);
    }

    private void OnDisplaySetting()
    {
        Debug.Log("[MainMenuUI] OnDisplaySetting | Display setting");
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        PopupManager.Instance.OpenPopup<SettingPopup>(PopupManager.PopupType.Setting);
    }

    private void OnExitGame()
    {
        Debug.Log("[MainMenuUI] OnExitGame | Exit game");
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        Application.Quit();
    }
}
