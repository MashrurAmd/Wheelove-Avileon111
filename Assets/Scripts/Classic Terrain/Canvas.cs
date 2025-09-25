using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Needed for scene loading

public class Canvas : MonoBehaviour
{


    public void LoadMainMenu()
    { 
               SceneManager.LoadScene(0); 
    }
}