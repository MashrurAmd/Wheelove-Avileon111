using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public enum Language
    {
        English,
        Hebrew,
        Russian,
        Arabic,
        Amharic, // ← add this

        ENG,
        Chineese,
        Spanish

    }

    public static Language currentLanguage = Language.English;

    public void SetEnglish() { currentLanguage = Language.English; RefreshAll(); }
    public void SetHebrew() { currentLanguage = Language.Hebrew; RefreshAll(); }
    public void SetRussian() { currentLanguage = Language.Russian; RefreshAll(); }
    public void SetArabic() { currentLanguage = Language.Arabic; RefreshAll(); }
    public void SetAmharic() { currentLanguage = Language.Amharic; RefreshAll(); } // ← add this

    public void SetEng() { currentLanguage = Language.ENG; RefreshAll(); }
    public void SetChineese() { currentLanguage = Language.Chineese; RefreshAll(); }
    public void SetSpanish() { currentLanguage = Language.Spanish; RefreshAll(); }

    void RefreshAll()
    {
        LocalizedText[] allTexts = FindObjectsOfType<LocalizedText>();
        foreach (var t in allTexts)
            t.Refresh();
    }
}