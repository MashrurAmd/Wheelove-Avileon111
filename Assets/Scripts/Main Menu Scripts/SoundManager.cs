using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("Button Icons")]
    public Image musicOnIcon;
    public Image musicOffIcon;
    public Image soundEffectOnIcon;
    public Image soundEffectOffIcon;

    [Header("Button Texts")]
    public TMP_Text musicButtonText;
    public TMP_Text soundEffectButtonText;

    private bool muted = false;
    private bool soundMuted = false;

    [Header("Volume Control")]
    public Slider volumeSlider;

    [Header("Music Settings")]
    public AudioSource audioSource;

    [Header("Sound Effect Settings")]
    public AudioSource soundEffectSource;
    public AudioClip[] buttonClick;

    void Start()
    {
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        if (PlayerPrefs.HasKey("MusicMuted"))
        {
            muted = PlayerPrefs.GetInt("MusicMuted") == 1;
            audioSource.mute = muted;
        }

        if (PlayerPrefs.HasKey("SfxMuted"))
        {
            soundMuted = PlayerPrefs.GetInt("SfxMuted") == 1;
            soundEffectSource.mute = soundMuted;
        }

        if (PlayerPrefs.HasKey("Volume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume");
            volumeSlider.value = savedVolume;
            OnVolumeChanged(savedVolume);
        }
        else
        {
            OnVolumeChanged(volumeSlider.value);
        }

        UpdateMusicButtonIcon();
        UpdateSoundEffectButtonIcon();
        PlayerSettingsManager.Instance.Load();
    }

    void OnVolumeChanged(float value)
    {
        audioSource.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void OnMusicButtonPrees()
    {
        muted = !muted;
        audioSource.mute = muted;
        PlayerPrefs.SetInt("MusicMuted", muted ? 1 : 0);
        UpdateMusicButtonIcon();
    }

    private void UpdateMusicButtonIcon()
    {
        musicOnIcon.enabled = !muted;
        musicOffIcon.enabled = muted;

        //if (musicButtonText != null)
        //    musicButtonText.text = muted ? "OFF" : "ON";
    }

    public void OnSoundEffectButtonPrees()
    {
        soundMuted = !soundMuted;
        soundEffectSource.mute = soundMuted;
        PlayerPrefs.SetInt("SfxMuted", soundMuted ? 1 : 0);
        UpdateSoundEffectButtonIcon();
    }

    private void UpdateSoundEffectButtonIcon()
    {
        soundEffectOnIcon.enabled = !soundMuted;
        soundEffectOffIcon.enabled = soundMuted;

        //if (soundEffectButtonText != null)
        //{
        //    soundEffectButtonText.text = soundMuted ? "OFF" : "ON";
        //}
    }

    public void SoundEffectButton()
    {
        if (!soundMuted && buttonClick.Length > 0)
        {
            soundEffectSource.PlayOneShot(buttonClick[0], 0.5f);
        }
    }
}
