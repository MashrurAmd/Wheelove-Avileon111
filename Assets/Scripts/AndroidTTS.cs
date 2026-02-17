using UnityEngine;

public class AndroidTTS : MonoBehaviour
{
    private AndroidJavaObject tts;
    private AndroidJavaObject activity;
    private bool isReady = false;

    void Start()
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
                Debug.Log("TTS Initialized Successfully");
            })
        );
#endif
    }

    public void Speak(string text)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (tts != null && isReady)
        {
            tts.Call<int>("speak", text, 0, null, null);
        }
#endif
    }

    void OnDestroy()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (tts != null)
        {
            tts.Call("shutdown");
        }
#endif
    }

    // 🔥 Initialization Listener
    private class TTSInitListener : AndroidJavaProxy
    {
        private System.Action onInitCallback;

        public TTSInitListener(System.Action callback)
            : base("android.speech.tts.TextToSpeech$OnInitListener")
        {
            onInitCallback = callback;
        }

        public void onInit(int status)
        {
            if (status == 0) // SUCCESS
            {
                onInitCallback?.Invoke();
            }
        }
    }
}
