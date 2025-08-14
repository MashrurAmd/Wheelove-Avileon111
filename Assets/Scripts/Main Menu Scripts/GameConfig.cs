using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameRegion { Israel, USA }
public enum GameMode { Kids, General }

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
public class GameConfig : ScriptableObject
{
    public GameRegion currentRegion;
    public GameMode currentMode;
    public string language;

    public void Save()
    {
        PlayerPrefs.SetInt("Region", (int)currentRegion);
        PlayerPrefs.SetString("Language", language);
        PlayerPrefs.SetInt("Mode", (int)currentMode);
        PlayerPrefs.SetInt("HasLaunchedBefore", 1);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("HasLaunchedBefore"))
        {
            currentRegion = (GameRegion)PlayerPrefs.GetInt("Region");
            language = PlayerPrefs.GetString("Language");
            currentMode = (GameMode)PlayerPrefs.GetInt("Mode");
        }
    }

    public bool IsFirstLaunch()
    {
        return !PlayerPrefs.HasKey("HasLaunchedBefore");
    }
}


