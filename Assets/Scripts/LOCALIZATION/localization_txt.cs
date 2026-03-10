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

    [Header("TMP Fonts")]
    public TMP_FontAsset amharicFont;    // ← Noto Sans Ethiopic TMP
    public TMP_FontAsset defaultFont;    // ← your normal TMP font

    [Header("Legacy Fonts")]
    public Font amharicLegacyFont;       // ← Noto Sans Ethiopic legacy .ttf
    public Font defaultLegacyFont;       // ← your normal legacy font

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
        bool isAmharic = LocalizationManager.currentLanguage == LocalizationManager.Language.Amharic;

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
            // ← Swap TMP font
            if (isAmharic && amharicFont != null)
                tmpText.font = amharicFont;
            else if (!isAmharic && defaultFont != null)
                tmpText.font = defaultFont;

            tmpText.text = display;
            tmpText.isRightToLeftText = isRTL;
        }
        else if (uiText != null)
        {
            // ← Swap legacy font
            if (isAmharic && amharicLegacyFont != null)
                uiText.font = amharicLegacyFont;
            else if (!isAmharic && defaultLegacyFont != null)
                uiText.font = defaultLegacyFont;

            uiText.text = display;
        }
    }
}