using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static bool isHebrew = false;

    public void SetEnglish()
    {
        isHebrew = false;
        RefreshAll();
    }

    public void SetHebrew()
    {
        isHebrew = true;
        RefreshAll();
    }

    void RefreshAll()
    {
        LocalizedText[] allTexts = FindObjectsOfType<LocalizedText>();
        foreach (var t in allTexts)
            t.Refresh();
    }
}