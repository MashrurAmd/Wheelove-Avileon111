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
    public string amharicText;  // ← add this

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
                display = amharicText;  // ← Amharic is LTR, no RTL needed
                break;
            case LocalizationManager.Language.English:
            default:
                display = englishText;
                break;
        }

        if (tmpText != null)
        {
            tmpText.text = display;
            tmpText.isRightToLeftText = isRTL;
        }
        else if (uiText != null)
        {
            uiText.text = display;
        }
    }
}