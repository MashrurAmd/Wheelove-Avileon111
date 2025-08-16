using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RegionData
{
    public string regionName;
    public List<string> languages;
}

[CreateAssetMenu(fileName = "GameSettingsData", menuName = "Game/GameSettingsData")]
public class GameSettingsData : ScriptableObject
{
    public List<RegionData> regions;
    public List<string> modes;
}
