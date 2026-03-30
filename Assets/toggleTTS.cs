using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TTSToggleButton : MonoBehaviour
{
    public Sprite onSprite;
    public Sprite offSprite;
    public Image buttonImage;
    public Button button;

    private bool isTTSOn = true;

    void Start()
    {
        // ← Auto find if not assigned in Inspector
        if (button == null)
            button = GetComponent<Button>();

        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        if (buttonImage == null)
            buttonImage = GetComponentInChildren<Image>();

        // ← Set correct sprite on start
        UpdateSprite();
    }

    public void OnButtonPressed()
    {
        if (button != null)
            button.interactable = false;

        isTTSOn = !isTTSOn;

        if (AndroidTTS.instance != null)
        {
            if (isTTSOn)
                AndroidTTS.instance.EnableTTS();
            else
                AndroidTTS.instance.DisableTTS();
        }

        UpdateSprite();
        StartCoroutine(ReEnableButton());
    }

    void UpdateSprite()
    {
        if (buttonImage == null) return;
        if (onSprite == null || offSprite == null) return;

        buttonImage.overrideSprite = isTTSOn ? onSprite : offSprite;
        buttonImage.SetAllDirty();
    }

    private IEnumerator ReEnableButton()
    {
        yield return new WaitForSeconds(0.3f);
        if (button != null)
            button.interactable = true;
    }
}