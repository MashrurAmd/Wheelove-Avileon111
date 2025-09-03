using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager instance;
    public GameObject trafficlight;
    public Material trafficNormalMat, trafficAlertMat;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void BackToMainMenu(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    private System.Collections.IEnumerator ChangeTrafficLightToGreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Switch to green light (normal mat)
        GamePlayManager.instance.trafficlight.GetComponent<MeshRenderer>().material =
            GamePlayManager.instance.trafficNormalMat;
    }

}
