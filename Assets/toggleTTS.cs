using UnityEngine;
using UnityEngine.UI;

public class TTSToggleButton : MonoBehaviour
{
    public Sprite onSprite;
    public Sprite offSprite;
    public Image buttonImage;

    private bool isTTSOn = true;

    public void OnButtonPressed()
    {
        isTTSOn = !isTTSOn;

        if (AndroidTTS.instance != null)
        {
            if (isTTSOn)
                AndroidTTS.instance.EnableTTS();
            else
                AndroidTTS.instance.DisableTTS();
        }

        // ← Swap icon
        //if (buttonImage != null)
        //    buttonImage.sprite = isTTSOn ? onSprite : offSprite;
        if (buttonImage != null)
        {
            buttonImage.overrideSprite = isTTSOn ? onSprite : offSprite;
            buttonImage.SetAllDirty();
        }
    }
}