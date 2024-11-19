using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingInGamePopup : SettingPopup
{
    private readonly string MAIN_MENU_STRING = "MainMenu";
    private readonly string GAMEPLAY_STRING = "Gameplay";

    [Header("Buttons"), Space(6)]
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button newGameButton;

    protected override void RegisterEvents()
    {
        base.RegisterEvents();
        backToMenuButton.onClick.AddListener(OnBackToMenu);
        newGameButton.onClick.AddListener(OnNewGame);
    }

    protected override void UnRegisterEvents()
    {
        base.UnRegisterEvents();
        backToMenuButton.onClick.RemoveListener(OnBackToMenu);
        newGameButton.onClick.RemoveListener(OnNewGame);
    }

    private void OnBackToMenu()
    {
        PopupManager.Instance.CloseAllPopup();
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.MainMenuBackground);
        SceneManager.LoadScene(MAIN_MENU_STRING);
    }

    private void OnNewGame()
    {
        Enviroment.Instance.ResetGame();
        PopupManager.Instance.CloseAllPopup();
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.GameplayBackground);
        SceneManager.LoadScene(GAMEPLAY_STRING);
    }
}
