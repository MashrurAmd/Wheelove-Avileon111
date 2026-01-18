//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class SoundManager : MonoBehaviour
//{
//    public static SoundManager Instance;

//    [Header("Music Data ScriptableObject)")]
//    public TestMusicData testMusicData;

//    [Header("Button Icons")]
//    public Image musicOnIcon;
//    public Image musicOffIcon;
//    public Image soundEffectOnIcon;
//    public Image soundEffectOffIcon;

//    [Header("Button Texts")]
//    public TMP_Text musicButtonText;
//    public TMP_Text soundEffectButtonText;

//    private bool muted = false;
//    private bool soundMuted = false;

//    [Header("Volume Control")]
//    public Slider volumeSlider;

//    [Header("Music Settings")]
//    public AudioSource audioSource;

//    [Header("Sound Effect Settings")]
//    public AudioSource soundEffectSource;
//    public AudioClip[] buttonClick;

//    //void Awake()
//    //{
//    //    if (FindObjectsOfType<SoundManager>().Length > 1)
//    //    {
//    //        Destroy(gameObject);
//    //        return;
//    //    }

//    //    DontDestroyOnLoad(gameObject);
//    //}

//    void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        Instance = this;
//        DontDestroyOnLoad(gameObject);
//    }


//    void Start()
//    {
//        if (audioSource == null)
//            audioSource = GetComponent<AudioSource>();

//        if (soundEffectSource == null)
//            soundEffectSource = GetComponent<AudioSource>();

//#if UNITY_ANDROID
//        PlayerPrefs.DeleteKey("MusicMuted");
//        PlayerPrefs.DeleteKey("SfxMuted");
//        PlayerPrefs.DeleteKey("Volume");
//#endif


//        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

//        if (PlayerPrefs.HasKey("MusicMuted"))
//        {
//            muted = PlayerPrefs.GetInt("MusicMuted") == 1;
//            audioSource.mute = muted;
//        }

//        if (PlayerPrefs.HasKey("SfxMuted"))
//        {
//            soundMuted = PlayerPrefs.GetInt("SfxMuted") == 1;
//            soundEffectSource.mute = soundMuted;
//        }

//        if (PlayerPrefs.HasKey("Volume"))
//        {
//            float savedVolume = PlayerPrefs.GetFloat("Volume");
//            volumeSlider.value = savedVolume;
//            OnVolumeChanged(savedVolume);
//        }
//        else
//        {
//            OnVolumeChanged(volumeSlider.value);
//        }


//        if (audioSource == null)
//            audioSource = GetComponent<AudioSource>();

//        if (soundEffectSource == null)
//            soundEffectSource = GetComponent<AudioSource>();



//        UpdateMusicButtonIcon();
//        UpdateSoundEffectButtonIcon();
//        PlayerSettingsManager.Instance.Load();

//        PlayMainMenuMusic();

//    }

//    void OnVolumeChanged(float value)
//    {
//        audioSource.volume = value;
//        soundEffectSource.volume = value;
//        PlayerPrefs.SetFloat("Volume", value);
//    }

//    public void OnMusicButtonPrees()
//    {
//        muted = !muted;
//        audioSource.mute = muted;
//        PlayerPrefs.SetInt("MusicMuted", muted ? 1 : 0);
//        UpdateMusicButtonIcon();
//    }

//    private void UpdateMusicButtonIcon()
//    {
//        musicOnIcon.enabled = !muted;
//        musicOffIcon.enabled = muted;

//        if (musicButtonText != null)
//            musicButtonText.text = muted ? "OFF" : "ON";
//    }

//    public void OnSoundEffectButtonPrees()
//    {
//        soundMuted = !soundMuted;
//        soundEffectSource.mute = soundMuted;
//        PlayerPrefs.SetInt("SfxMuted", soundMuted ? 1 : 0);
//        UpdateSoundEffectButtonIcon();
//    }

//    private void UpdateSoundEffectButtonIcon()
//    {
//        soundEffectOnIcon.enabled = !soundMuted;
//        soundEffectOffIcon.enabled = soundMuted;

//        if (soundEffectButtonText != null)
//        {
//            soundEffectButtonText.text = soundMuted ? "OFF" : "ON";
//        }
//    }

//    public void SoundEffectButton()
//    {
//        if (!soundMuted && buttonClick.Length > 0)
//        {
//            //soundEffectSource.PlayOneShot(buttonClick[0], 0.5f);

//            //soundEffectSource.mute = false;
//            soundEffectSource.PlayOneShot(buttonClick[0], 0.5f);
//        }
//    }

//    public void PlayMainMenuMusic()
//    {
//        audioSource.Stop();
//        audioSource.clip = testMusicData.mainMenuMusic;
//        audioSource.loop = true;
//        audioSource.Play(); 
//    }

//    public void PlayGameplayMusic()
//    {
//        audioSource.Stop();
//        audioSource.clip = testMusicData.gameplayMusic;
//        audioSource.loop = true;
//        audioSource.Play();
//    }


//    public void PlaySFX(string sfxName)
//    {
//        if (soundMuted || testMusicData == null) return;

//        AudioEntry entry = testMusicData.soundEffects.Find(x => x.audioName == sfxName);

//        if (entry.clip != null)
//        {
//            soundEffectSource.PlayOneShot(entry.clip);
//        }
//        else
//        {
//            Debug.LogWarning("SFX not found: " + sfxName);
//        }
//    }

//    void Reset()
//    {
//        AudioSource[] sources = GetComponents<AudioSource>();
//        if (sources.Length >= 2)
//        {
//            audioSource = sources[0];
//            soundEffectSource = sources[1];
//        }
//    }


//}

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

        PlayMainMenuMusic();
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

    public void PlayMainMenuMusic()
    {
        musicSource.Stop();
        musicSource.clip = testMusicData.mainMenuMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayGameplayMusic()
    {
        musicSource.Stop();
        musicSource.clip = testMusicData.gameplayMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    // ✅ DATA-DRIVEN SFX SYSTEM
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
