using UnityEngine;
using UnityEngine.UI;

public class AndroidTTS : MonoBehaviour
{
    public static AndroidTTS instance;

    private AndroidJavaObject tts;
    private AndroidJavaObject activity;
    private bool isReady = false;

    //private bool isEnabled = false;
    private bool isEnabled = true;

    [Header("UI Buttons")]
    public Button onButton;
    public Button offButton;

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
        }
    }

    void Start()
    {
        UpdateButtonState(); // default OFF

#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        tts = new AndroidJavaObject("android.speech.tts.TextToSpeech", activity,
            new TTSInitListener(() =>
            {
                isReady = true;
            })
        );
#endif
    }

    // 🔥 ON Button Press
    public void EnableTTS()
    {
        isEnabled = true;
        UpdateButtonState();

        // Immediately read current question
        QuestionManager qm = FindObjectOfType<QuestionManager>();
        if (qm != null)
        {
            qm.ReadCurrentQuestion();
        }
    }

    // 🔥 OFF Button Press
    public void DisableTTS()
    {
        isEnabled = false;
        Stop();
        UpdateButtonState();
    }

    public void Speak(string text)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (tts != null && isReady && isEnabled)
        {
            tts.Call<int>("speak", text, 0, null, null);
        }
#endif
    }

    public void Stop()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (tts != null)
        {
            tts.Call("stop");
        }
#endif
    }

    private void UpdateButtonState()
    {
        if (onButton != null)
            onButton.gameObject.SetActive(!isEnabled);

        if (offButton != null)
            offButton.gameObject.SetActive(isEnabled);
    }

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
}

////////////////////////////////////////////////////

//using UnityEngine;
//using UnityEngine.UI;

//public class AndroidTTS : MonoBehaviour
//{
//    public static AndroidTTS instance;

//    private AndroidJavaObject tts;
//    private AndroidJavaObject activity;
//    private bool isReady = false;

//    private bool isEnabled = false;

//    [Header("UI Buttons")]
//    public Button onButton;   // Visible when TTS is OFF
//    public Button offButton;  // Visible when TTS is ON

//    void Awake()
//    {
//        if (instance == null)
//        {
//            instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//            return;
//        }
//    }

//    void Start()
//    {
//        // 🔥 Default state = OFF
//        isEnabled = false;
//        UpdateButtonState();

//        tts.Call<int>("setSpeechRate", 0.85f);

//#if UNITY_ANDROID && !UNITY_EDITOR
//        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//        {
//            activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
//        }

//        tts = new AndroidJavaObject("android.speech.tts.TextToSpeech",
//            activity,
//            new TTSInitListener(() =>
//            {
//                isReady = true;
//            })
//        );
//#endif
//    }

//    // 🔊 Enable TTS
//    public void EnableTTS()
//    {
//        isEnabled = true;
//        UpdateButtonState();

//        // Immediately read current question
//        QuestionManager qm = FindObjectOfType<QuestionManager>();
//        if (qm != null)
//        {
//            qm.ReadCurrentQuestion();
//        }
//    }

//    // 🔇 Disable TTS
//    public void DisableTTS()
//    {
//        isEnabled = false;
//        Stop();
//        UpdateButtonState();
//    }

//    public void Speak(string text)
//    {
//#if UNITY_ANDROID && !UNITY_EDITOR
//        if (tts != null && isReady && isEnabled)
//        {
//            tts.Call<int>("speak", text, 0, null, null);
//        }
//#endif
//    }

//    public void Stop()
//    {
//#if UNITY_ANDROID && !UNITY_EDITOR
//        if (tts != null)
//        {
//            tts.Call("stop");
//        }
//#endif
//    }

//    private void UpdateButtonState()
//    {
//        if (onButton != null)
//            onButton.gameObject.SetActive(!isEnabled);  // Show ON when disabled

//        if (offButton != null)
//            offButton.gameObject.SetActive(isEnabled);  // Show OFF when enabled
//    }

//    private class TTSInitListener : AndroidJavaProxy
//    {
//        private System.Action callback;

//        public TTSInitListener(System.Action cb)
//            : base("android.speech.tts.TextToSpeech$OnInitListener")
//        {
//            callback = cb;
//        }

//        public void onInit(int status)
//        {
//            if (status == 0)
//                callback?.Invoke();
//        }
//    }
//}
