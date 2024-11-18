using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public enum SoundType
    {
        //UI, background
        Background = 0,
        ButtonClick = 1,
        Popup = 2,
        Popdown = 3,
    }

    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private Transform audioEffectHolder;

    private float volumeAudioEffect = 1;

    public float EffectVolume => volumeAudioEffect;
    public float BackgroundVolume => backgroundAudioSource.volume;

    private const string BackgroundMusicKey = "BackgroundMusic";
    private const string EffectMusicKey = "EffectMusic";

    private const string BackgroundVolumeKey = "BackgroundVolume";
    private const string EffectVolumeKey = "EffectVolume";

    protected override void Awake()
    {
        base.Awake();
        LoadSettings();
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        PlayAudio(SoundType.Background);
    }

    public void MuteBackgroundAudio(bool isMute)
    {
        if (isMute) backgroundAudioSource.volume = 0;
        else backgroundAudioSource.volume = PlayerPrefs.GetFloat(BackgroundMusicKey, 1f);

        PlayerPrefs.SetInt(BackgroundMusicKey, isMute ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void MuteEffectAudio(bool isMute)
    {
        if (isMute) volumeAudioEffect = 0;
        else volumeAudioEffect = PlayerPrefs.GetFloat(EffectMusicKey, 1f);

        PlayerPrefs.SetInt(EffectMusicKey, isMute ? 1 : 0);
        PlayerPrefs.Save();
    }

    public AudioClip GetPopup(SoundType soundType)
    {
        return audioClips[(int)soundType];
    }

    public void PlayAudio(SoundType soundType)
    {
        AudioClip clip = GetPopup(soundType);

        if (soundType == SoundType.Background)
        {
            backgroundAudioSource.clip = clip;
            backgroundAudioSource.Play();
            return;
        }
        PlaySoundEffect(clip);
    }

    public void ChangeEffectVolume(float volume)
    {
        volumeAudioEffect = volume;
        PlayerPrefs.SetFloat(EffectMusicKey, volumeAudioEffect);
        PlayerPrefs.Save();
    }

    public void ChangeBackgroundVolume(float volume)
    {
        backgroundAudioSource.volume = volume;
        PlayerPrefs.SetFloat(BackgroundMusicKey, backgroundAudioSource.volume); 
        PlayerPrefs.Save();
    }

    private void PlaySoundEffect(AudioClip clip)
    {
        AudioSource audioSource = new GameObject("SFX_Audio").AddComponent<AudioSource>();
        audioSource.transform.SetParent(audioEffectHolder);
        audioSource.clip = clip;
        audioSource.volume = volumeAudioEffect;
        audioSource.Play();
        Destroy(audioSource.gameObject, clip.length);
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey(BackgroundMusicKey))
        {
            bool isMuteBackground = PlayerPrefs.GetInt(BackgroundMusicKey) == 1;
            MuteBackgroundAudio(isMuteBackground);
        }

        if (PlayerPrefs.HasKey(EffectMusicKey))
        {
            bool isMuteSFX = PlayerPrefs.GetInt(EffectMusicKey) == 1;
            MuteEffectAudio(isMuteSFX);
        }

        if (PlayerPrefs.HasKey(BackgroundVolumeKey))
        {
            backgroundAudioSource.volume = PlayerPrefs.GetFloat(BackgroundVolumeKey, 1f); 
        }

        if (PlayerPrefs.HasKey(EffectVolumeKey))
        {
            volumeAudioEffect = PlayerPrefs.GetFloat(EffectVolumeKey, 1f); 
        }
    }
}
