using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [Header("English")]
    public string englishText;

    [Header("Hebrew")]
    public string hebrewText;

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
        // Every time this panel becomes active, refresh with current language
        Refresh();
    }

    public void Refresh()
    {
        string display = LocalizationManager.isHebrew ? hebrewText : englishText;

        if (tmpText != null) tmpText.text = display;
        else if (uiText != null) uiText.text = display;
    }
}