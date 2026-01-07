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

    [Header("Car Trigger")]
    public TriggerZone carTriggerZone;
    public float delayBeforeGreen = 10f;

    private enum LightState { Red, Yellow, Green }
    private LightState currentState;

    private Coroutine trafficRoutine;
    private Coroutine delayedGreenRoutine;

    void Start()
    {
        // 🔴 Start with RED
        SetLight(LightState.Red);
        trafficRoutine = StartCoroutine(TrafficLightRoutine());
    }

    void Update()
    {
        // 🚗 Car enters area
        if (carTriggerZone.isTriggered && delayedGreenRoutine == null)
        {
            delayedGreenRoutine = StartCoroutine(TurnGreenAfterDelay());
        }

        // 🚗 Car leaves area → cancel delayed green
        if (!carTriggerZone.isTriggered && delayedGreenRoutine != null)
        {
            StopCoroutine(delayedGreenRoutine);
            delayedGreenRoutine = null;
        }
    }

    IEnumerator TurnGreenAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeGreen);

        if (!carTriggerZone.isTriggered)
        {
            delayedGreenRoutine = null;
            yield break;
        }

        // 🟡 RED → YELLOW
        SetLight(LightState.Yellow);
        yield return new WaitForSeconds(1f);

        // 🟢 YELLOW → GREEN
        SetLight(LightState.Green);

        delayedGreenRoutine = null;
    }

    IEnumerator TrafficLightRoutine()
    {
        while (true)
        {
            // GREEN
            yield return new WaitUntil(() => currentState == LightState.Green);
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

        redLight.material = inactiveMat;
        yellowLight.material = inactiveMat;
        greenLight.material = inactiveMat;

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

    public bool IsRedLight()
    {
        return currentState == LightState.Red;
    }
}
