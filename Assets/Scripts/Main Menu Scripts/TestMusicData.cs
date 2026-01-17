using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AudioEntry
{
    public string audioName; 
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "TestMusicData", menuName = "Audio/TestMusicData")]
public class TestMusicData : ScriptableObject
{
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic;
    public List<AudioEntry> soundEffects;
}