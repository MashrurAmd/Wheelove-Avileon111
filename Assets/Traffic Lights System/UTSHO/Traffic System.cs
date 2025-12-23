using System.Collections;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    [Header("Light Mesh Renderers")]
    public MeshRenderer redLight;
    public MeshRenderer yellowLight;
    public MeshRenderer greenLight;

    [Header("Materials")]
    public Material redMat;
    public Material yellowMat;
    public Material greenMat;
    public Material inactiveMat;

    [Header("Timings")]
    public float greenTime = 5f;
    public float yellowTime = 2f;
    public float redTime = 5f;

    private enum LightState { Red, Yellow, Green }
    private LightState currentState;

    void Start()
    {
        StartCoroutine(TrafficLightRoutine());
    }

    IEnumerator TrafficLightRoutine()
    {
        while (true)
        {
            // GREEN
            SetLight(LightState.Green);
            yield return new WaitForSeconds(greenTime);

            // YELLOW
            SetLight(LightState.Yellow);
            yield return new WaitForSeconds(yellowTime);

            // RED
            SetLight(LightState.Red);
            yield return new WaitForSeconds(redTime);
        }
    }

    void SetLight(LightState state)
    {
        currentState = state;

        // Turn all lights inactive first
        redLight.material = inactiveMat;
        yellowLight.material = inactiveMat;
        greenLight.material = inactiveMat;

        // Activate only one
        switch (state)
        {
            case LightState.Red:
                redLight.material = redMat;
                break;

            case LightState.Yellow:
                yellowLight.material = yellowMat;
                break;

            case LightState.Green:
                greenLight.material = greenMat;
                break;
        }
    }

    // 🔴 FOR FUTURE CAR SYSTEM
    public bool IsRedLight()
    {
        return currentState == LightState.Red;
    }
}
