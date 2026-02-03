using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Data")]
    public TestMusicData testMusicData;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("UI")]
    public Slider volumeSlider;
    public Image musicOnIcon, musicOffIcon;
    public Image sfxOnIcon, sfxOffIcon;
    public TMP_Text musicButtonText;
    public TMP_Text sfxButtonText;

    private bool musicMuted = false;
    private bool sfxMuted = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // Load settings
        musicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        sfxMuted = PlayerPrefs.GetInt("SfxMuted", 0) == 1;

        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);

        musicSource.mute = musicMuted;
        sfxSource.mute = sfxMuted;

        UpdateMusicUI();
        UpdateSfxUI();

        //PlayMainMenuMusic();
        PlayMusic("MainMenu");
    }

    public void SetVolume(float value)
    {
        musicSource.volume = value;
        sfxSource.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void ToggleMusic()
    {
        musicMuted = !musicMuted;
        musicSource.mute = musicMuted;
        PlayerPrefs.SetInt("MusicMuted", musicMuted ? 1 : 0);
        UpdateMusicUI();
    }

    public void ToggleSfx()
    {
        sfxMuted = !sfxMuted;
        sfxSource.mute = sfxMuted;
        PlayerPrefs.SetInt("SfxMuted", sfxMuted ? 1 : 0);
        UpdateSfxUI();
    }

    void UpdateMusicUI()
    {
        musicOnIcon.enabled = !musicMuted;
        musicOffIcon.enabled = musicMuted;
        musicButtonText.text = musicMuted ? "OFF" : "ON";
    }

    void UpdateSfxUI()
    {
        sfxOnIcon.enabled = !sfxMuted;
        sfxOffIcon.enabled = sfxMuted;
        sfxButtonText.text = sfxMuted ? "OFF" : "ON";
    }

    //public void PlayMainMenuMusic()
    //{
    //    musicSource.Stop();
    //    musicSource.clip = testMusicData.mainMenuMusic;
    //    musicSource.loop = true;
    //    musicSource.Play();
    //}

    //public void PlayGameplayMusic()
    //{
    //    musicSource.Stop();
    //    musicSource.clip = testMusicData.gameplayMusic;
    //    musicSource.loop = true;
    //    musicSource.Play();
    //}

    public void PlayMusic(string musicName)
    {
        if (musicMuted || testMusicData == null) return;

        AudioEntry entry = testMusicData.allMusic
            .Find(x => x.audioName == musicName);

        if (entry != null && entry.clip != null)
        {
            // Prevent restarting same music
            if (musicSource.clip == entry.clip && musicSource.isPlaying)
                return;

            musicSource.Stop();
            musicSource.clip = entry.clip;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music not found: " + musicName);
        }
    }


    // DATA-DRIVEN SFX SYSTEM
    public void PlaySFX(string sfxName)
    {
        if (sfxMuted || testMusicData == null) return;

        AudioEntry entry = testMusicData.soundEffects
            .Find(x => x.audioName == sfxName);

        if (entry != null && entry.clip != null)
        {
            sfxSource.PlayOneShot(entry.clip);
        }
        else
        {
            Debug.LogWarning("SFX not found: " + sfxName);
        }
    }

    // For UI button click
    public void ButtonClickSFX()
    {
        PlaySFX("ButtonClick");
    }
}
