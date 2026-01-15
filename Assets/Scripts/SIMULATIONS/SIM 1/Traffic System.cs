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

    // 🔴🟡🟢 BOOL STATES
    [Header("Current State (Read Only)")]
    public bool isRed;
    public bool isYellow;
    public bool isGreen;

    private Coroutine trafficRoutine;
    private Coroutine delayedGreenRoutine;

    void Start()
    {
        SetRed();
        trafficRoutine = StartCoroutine(TrafficLightRoutine());
    }

    void Update()
    {
        // 🚗 Car enters trigger → prepare green
        if (carTriggerZone.isTriggered && delayedGreenRoutine == null)
        {
            delayedGreenRoutine = StartCoroutine(TurnGreenAfterDelay());
        }

        // 🚗 Car leaves → cancel delayed green
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

        SetYellow();
        yield return new WaitForSeconds(1f);

        SetGreen();
        delayedGreenRoutine = null;
    }

    IEnumerator TrafficLightRoutine()
    {
        while (true)
        {
            // wait until green is active
            yield return new WaitUntil(() => isGreen);
            yield return new WaitForSeconds(greenTime);

            SetYellow();
            yield return new WaitForSeconds(yellowTime);

            SetRed();
            yield return new WaitForSeconds(redTime);
        }
    }

    // =========================
    // LIGHT STATE SETTERS
    // =========================

    void SetRed()
    {
        isRed = true;
        isYellow = false;
        isGreen = false;

        redLight.material = redMat;
        yellowLight.material = inactiveMat;
        greenLight.material = inactiveMat;
    }

    void SetYellow()
    {
        isRed = false;
        isYellow = true;
        isGreen = false;

        redLight.material = inactiveMat;
        yellowLight.material = yellowMat;
        greenLight.material = inactiveMat;
    }

    void SetGreen()
    {
        isRed = false;
        isYellow = false;
        isGreen = true;

        redLight.material = inactiveMat;
        yellowLight.material = inactiveMat;
        greenLight.material = greenMat;
    }

    // =========================
    // PUBLIC CHECK HELPERS
    // =========================

    public bool IsRedLight()
    {
        return isRed;
    }

    public bool IsGreenLight()
    {
        return isGreen;
    }

    public bool IsYellowLight()
    {
        return isYellow;
    }
}
