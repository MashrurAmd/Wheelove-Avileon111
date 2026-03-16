using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Collections;

public class AndroidTTS : MonoBehaviour
{
    public static AndroidTTS instance;

    private bool isReady = false;
    private bool isEnabled = true;

    [Header("UI Buttons — Assign in Inspector!")]
    public Button onButton;
    public Button offButton;

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _IOSSpeak(string text);

    [DllImport("__Internal")]
    private static extern void _IOSStop();
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject tts;
    private AndroidJavaObject activity;
#endif

    // =====================
    // UNITY EVENTS
    // =====================

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        InitTTS();
        WireUpButtons();
        UpdateButtonState();
    }

    // =====================
    // INIT
    // =====================

    private void InitTTS()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        tts = new AndroidJavaObject("android.speech.tts.TextToSpeech", activity,
            new TTSInitListener(() =>
            {
                isReady = true;
                Debug.Log("TTS Ready ✅");
            })
        );
#elif UNITY_IOS && !UNITY_EDITOR
        isReady = true;
#else
        isReady = true; // Editor
#endif
    }

    // =====================
    // BUTTON WIRING
    // =====================

    private void WireUpButtons()
    {
        if (onButton != null)
        {
            onButton.onClick.RemoveAllListeners();
            onButton.onClick.AddListener(EnableTTS);
            Debug.Log("ON button wired ✅");
        }
        else
        {
            Debug.LogWarning("[AndroidTTS] ⚠️ onButton is NOT assigned in Inspector!");
        }

        if (offButton != null)
        {
            offButton.onClick.RemoveAllListeners();
            offButton.onClick.AddListener(DisableTTS);
            Debug.Log("OFF button wired ✅");
        }
        else
        {
            Debug.LogWarning("[AndroidTTS] ⚠️ offButton is NOT assigned in Inspector!");
        }
    }

    public bool IsEnabled()
    {
        return isEnabled;
    }

    // =====================
    // PUBLIC CONTROLS
    // =====================

    //public void EnableTTS()
    //{
    //    isEnabled = true;
    //    UpdateButtonState();

    //    QuestionManager qm = FindObjectOfType<QuestionManager>();
    //    if (qm != null)
    //        qm.ReadCurrentQuestion();

    //    Debug.Log("TTS Enabled ✅");
    //}

    //public void DisableTTS()
    //{
    //    isEnabled = false;
    //    Stop();
    //    UpdateButtonState();
    //    Debug.Log("TTS Disabled 🔇");
    //}
    public void EnableTTS()
    {
        isEnabled = true;
        UpdateButtonState();
        Debug.Log("TTS Enabled ✅");

        // Re-speak current question + options
        StartCoroutine(SpeakAfterDelay());
    }

    private IEnumerator SpeakAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);
        QuestionManager qm = FindObjectOfType<QuestionManager>();
        if (qm != null)
            qm.ReadCurrentQuestion();
        else
            Debug.LogWarning("[AndroidTTS] QuestionManager not found!");
    }

    public void DisableTTS()
    {
        isEnabled = false;
        Stop();
        UpdateButtonState();
        Debug.Log("TTS Disabled 🔇");
    }

    public void Speak(string text)
    {
        if (!isEnabled || !isReady || string.IsNullOrEmpty(text)) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        tts?.Call<int>("speak", text, 0, null, null);
#elif UNITY_IOS && !UNITY_EDITOR
        _IOSSpeak(text);
#else
        Debug.Log($"[TTS Editor] {text}");
#endif
    }

    public void Stop()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        tts?.Call("stop");
#elif UNITY_IOS && !UNITY_EDITOR
        _IOSStop();
#endif
    }

    // =====================
    // UI STATE
    // =====================

    private void UpdateButtonState()
    {
        // OnButton  → visible when TTS is OFF (press to turn ON)
        if (onButton != null)
            onButton.gameObject.SetActive(!isEnabled);

        // OffButton → visible when TTS is ON (press to turn OFF)
        if (offButton != null)
            offButton.gameObject.SetActive(isEnabled);
    }

    // =====================
    // ANDROID TTS LISTENER
    // =====================

#if UNITY_ANDROID && !UNITY_EDITOR
    private class TTSInitListener : AndroidJavaProxy
    {
        private System.Action callback;

        public TTSInitListener(System.Action cb)
            : base("android.speech.tts.TextToSpeech$OnInitListener")
        {
            callback = cb;
        }

        public void onInit(int status)
        {
            if (status == 0)
                callback?.Invoke();
        }
    }
#endif
}