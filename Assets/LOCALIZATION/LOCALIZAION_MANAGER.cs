using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public enum Language
    {
        English,
        Hebrew,
        Russian,
        Arabic
    }

    public static Language currentLanguage = Language.English;

    public void SetEnglish()
    {
        currentLanguage = Language.English;
        RefreshAll();
    }

    public void SetHebrew()
    {
        currentLanguage = Language.Hebrew;
        RefreshAll();
    }

    public void SetRussian()
    {
        currentLanguage = Language.Russian;
        RefreshAll();
    }

    public void SetArabic()
    {
        currentLanguage = Language.Arabic;
        RefreshAll();
    }

    void RefreshAll()
    {
        LocalizedText[] allTexts = FindObjectsOfType<LocalizedText>();
        foreach (var t in allTexts)
            t.Refresh();
    }
}