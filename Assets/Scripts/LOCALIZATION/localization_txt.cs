using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [Header("English")]
    public string englishText;
    [Header("Hebrew")]
    public string hebrewText;
    [Header("Russian")]
    public string russianText;
    [Header("Arabic")]
    public string arabicText;
    [Header("Amharic")]
    public string amharicText;

    [Header("Fonts")]
    public TMP_FontAsset amharicFont;   // ← assign Noto Sans Ethiopic here
    public TMP_FontAsset defaultFont;   // ← assign your normal TMP font here

    private TMP_Text tmpText;
    private Text uiText;

    void Start()
    {
        tmpText = GetComponent<TMP_Text>();
        uiText = GetComponent<Text>();
        Refresh();
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        string display = englishText;
        bool isRTL = false;

        switch (LocalizationManager.currentLanguage)
        {
            case LocalizationManager.Language.Hebrew:
                display = hebrewText;
                isRTL = true;
                break;
            case LocalizationManager.Language.Russian:
                display = russianText;
                break;
            case LocalizationManager.Language.Arabic:
                display = arabicText;
                isRTL = true;
                break;
            case LocalizationManager.Language.Amharic:
                display = amharicText;
                break;
            case LocalizationManager.Language.English:
            default:
                display = englishText;
                break;
        }

        if (tmpText != null)
        {
            // ← Swap font for Amharic
            if (LocalizationManager.currentLanguage == LocalizationManager.Language.Amharic
                && amharicFont != null)
                tmpText.font = amharicFont;
            else if (defaultFont != null)
                tmpText.font = defaultFont;

            tmpText.text = display;
            tmpText.isRightToLeftText = isRTL;
        }
        else if (uiText != null)
        {
            uiText.text = display;
        }
    }
}