using UnityEngine;

[CreateAssetMenu(fileName = "TestMusicData", menuName = "Audio/TestMusicData")]
public class TestMusicData : ScriptableObject
{
    public AudioClip openingTheme;
    public AudioClip finalTheme;
    public AudioClip[] testMusics;   
}