using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : BasePopup
{
    [SerializeField] private UISwitcher.UISwitcher enableBackgroundMusicSwitcher;
    [SerializeField] private UISwitcher.UISwitcher enableSFXMusicSwitcher;

    [SerializeField] private Slider backgroundMusicSlider;
    [SerializeField] private Slider SFXMusicSlider;   

    private void Start()
    {
        enableBackgroundMusicSwitcher.SetWithoutNotify(AudioManager.Instance.BackgroundVolume != 0);
        enableSFXMusicSwitcher.SetWithoutNotify(AudioManager.Instance.EffectVolume != 0);

        backgroundMusicSlider.value = AudioManager.Instance.BackgroundVolume;
        SFXMusicSlider.value = AudioManager.Instance.EffectVolume;
        RegisterEvents();
    }

    protected virtual void RegisterEvents()
    {
        enableBackgroundMusicSwitcher.OnValueChanged += OnMuteBackgroundMusic;
        enableSFXMusicSwitcher.OnValueChanged += OnMuteEffectMusic;

        backgroundMusicSlider.onValueChanged.AddListener(UpdateBackgroundVolume);
        SFXMusicSlider.onValueChanged.AddListener(UpdateSoundEffectVolume);
    }

    private void OnDestroy()
    {
        UnRegisterEvents();
    }

    protected virtual void UnRegisterEvents()
    {
        enableBackgroundMusicSwitcher.OnValueChanged -= OnMuteBackgroundMusic;
        enableSFXMusicSwitcher.OnValueChanged -= OnMuteEffectMusic;

        backgroundMusicSlider.onValueChanged.RemoveListener(UpdateBackgroundVolume);
        SFXMusicSlider.onValueChanged.RemoveListener(UpdateSoundEffectVolume);
    }

    private void OnMuteBackgroundMusic(bool isMute)
    {
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        AudioManager.Instance.MuteBackgroundAudio(!isMute);
    }

    private void OnMuteEffectMusic(bool isMute)
    {
        AudioManager.Instance.PlayAudio(AudioManager.SoundType.ButtonClick);
        AudioManager.Instance.MuteEffectAudio(!isMute);
    }

    void UpdateBackgroundVolume(float value)
    {
        AudioManager.Instance.ChangeBackgroundVolume(value);
    }

    void UpdateSoundEffectVolume(float value)
    {
        AudioManager.Instance.ChangeEffectVolume(value);
    }
}
