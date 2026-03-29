//using UnityEngine;
//using UnityEngine.UI;

//public class TTSToggleButton : MonoBehaviour
//{
//    public Sprite onSprite;
//    public Sprite offSprite;
//    public Image buttonImage;

//    private bool isTTSOn = true;

//    public void OnButtonPressed()
//    {
//        isTTSOn = !isTTSOn;

//        if (AndroidTTS.instance != null)
//        {
//            if (isTTSOn)
//                AndroidTTS.instance.EnableTTS();
//            else
//                AndroidTTS.instance.DisableTTS();
//        }

//        // ← Swap icon
//        //if (buttonImage != null)
//        //    buttonImage.sprite = isTTSOn ? onSprite : offSprite;
//        if (buttonImage != null)
//        {
//            buttonImage.overrideSprite = isTTSOn ? onSprite : offSprite;
//            buttonImage.SetAllDirty();
//        }
//    }
//}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TTSToggleButton : MonoBehaviour
{
    public Sprite onSprite;
    public Sprite offSprite;
    public Image buttonImage;
    public Button button;   // ← add this

    private bool isTTSOn = true;

    public void OnButtonPressed()
    {
        button.interactable = false;   // prevent spam clicking

        isTTSOn = !isTTSOn;

        if (AndroidTTS.instance != null)
        {
            if (isTTSOn)
                AndroidTTS.instance.EnableTTS();
            else
                AndroidTTS.instance.DisableTTS();
        }

        if (buttonImage != null)
        {
            buttonImage.overrideSprite = isTTSOn ? onSprite : offSprite;
            buttonImage.SetAllDirty();
        }

        StartCoroutine(ReEnableButton());
    }

    private IEnumerator ReEnableButton()
    {
        yield return new WaitForSeconds(0.3f);
        button.interactable = true;
    }
}