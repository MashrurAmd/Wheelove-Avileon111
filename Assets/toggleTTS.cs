using UnityEngine;
using UnityEngine.UI;

public class TTSToggleButton : MonoBehaviour
{
    public Sprite onSprite;
    public Sprite offSprite;
    public Image buttonImage;

    private bool isTTSOn = true;

    void Start()
    {
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();
        if (buttonImage == null)
            buttonImage = GetComponentInChildren<Image>();

        // ← Load saved state
        isTTSOn = PlayerPrefs.GetInt("TTSEnabled", 1) == 1;

        if (AndroidTTS.instance != null)
        {
            if (isTTSOn) AndroidTTS.instance.EnableTTS();
            else AndroidTTS.instance.DisableTTS();
        }

        UpdateSprite();
    }

    public void OnButtonPressed()
    {
        isTTSOn = !isTTSOn;

        // ← Save state
        PlayerPrefs.SetInt("TTSEnabled", isTTSOn ? 1 : 0);
        PlayerPrefs.Save();

        if (AndroidTTS.instance != null)
        {
            if (isTTSOn)
                AndroidTTS.instance.EnableTTS();
            else
                AndroidTTS.instance.DisableTTS();
        }

        UpdateSprite();
    }

    void UpdateSprite()
    {
        if (buttonImage == null) return;
        if (onSprite == null || offSprite == null) return;

        // ← Use sprite directly NOT overrideSprite
        buttonImage.sprite = isTTSOn ? onSprite : offSprite;

        // ← Force Android to redraw
        buttonImage.enabled = false;
        buttonImage.enabled = true;
    }
}