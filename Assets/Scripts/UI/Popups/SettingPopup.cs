using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

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

    private void RegisterEvents()
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

    private void UnRegisterEvents()
    {
        enableBackgroundMusicSwitcher.OnValueChanged -= OnMuteBackgroundMusic;
        enableSFXMusicSwitcher.OnValueChanged -= OnMuteEffectMusic;

        backgroundMusicSlider.onValueChanged.RemoveListener(UpdateBackgroundVolume);
        SFXMusicSlider.onValueChanged.RemoveListener(UpdateSoundEffectVolume);
    }

    private void OnMuteBackgroundMusic(bool isMute)
    {
        AudioManager.Instance.MuteBackgroundAudio(!isMute);
    }

    private void OnMuteEffectMusic(bool isMute)
    {
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
