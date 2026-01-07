using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StopSimulationController : MonoBehaviour
{
    [Header("References")]
    public Car car;                   // Reference to your car
    public TriggerZone stopArea;      // Trigger area where car must stop
    public Image signalImage;         // UI image for red/green indicator
    public Sprite greenSprite;        // Green light sprite
    public Sprite redSprite;          // Red light sprite

    [Header("Blink Settings")]
    public float blinkSpeed = 0.5f;     // how fast the red blinks
    public float dimAlpha = 0.3f;
    public float brightAlpha = 1f;
    public float blinkDuration = 5f;    // max blinking duration

    [Header("Stop Settings")]
    public float stopThreshold = 0.05f; // speed below which car is considered stopped

    private bool hasStoppedInside = false;  // Did the car stop while inside
    private bool wasInside = false;         // Did the car enter the area
    private bool isBlinking = false;
    private bool hasBlinkedThisEntry = false;
    private Coroutine blinkRoutine;

    void Update()
    {
        if (stopArea == null || car == null || signalImage == null)
            return; // safety check

        // 🚗 Car inside stop zone
        if (stopArea.isTriggered)
        {
            // start red blink
            if (!hasBlinkedThisEntry)
                StartRedBlink();

            wasInside = true;

            // check if car stopped
            if (car.CurrentSpeed <= stopThreshold)
            {
                if (!hasStoppedInside)
                    Debug.Log("Car STOPPED inside the area");

                hasStoppedInside = true;
            }
        }
        else
        {
            // stop blinking and set green
            ResetState();

            // 🚨 Car just left the area → evaluate
            if (wasInside)
            {
                if (hasStoppedInside)
                    Debug.Log("Successfully stopped at the stop sign 👍");
                else
                    Debug.Log("Rule broken — car did NOT stop ❌");
            }

            // reset flags for next attempt
            wasInside = false;
            hasStoppedInside = false;
        }
    }

    void StartRedBlink()
    {
        isBlinking = true;
        hasBlinkedThisEntry = true;
        blinkRoutine = StartCoroutine(BlinkEffect());
    }

    void ResetState()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        isBlinking = false;
        hasBlinkedThisEntry = false;

        signalImage.sprite = greenSprite;
        signalImage.color = Color.white;
    }

    IEnumerator BlinkEffect()
    {
        float elapsed = 0f;
        signalImage.sprite = redSprite;

        while (elapsed < blinkDuration && stopArea.isTriggered)
        {
            signalImage.color = new Color(1f, 1f, 1f, brightAlpha);
            yield return new WaitForSeconds(blinkSpeed);
            elapsed += blinkSpeed;

            signalImage.color = new Color(1f, 1f, 1f, dimAlpha);
            yield return new WaitForSeconds(blinkSpeed);
            elapsed += blinkSpeed;
        }

        // finalize red sprite fully visible if still inside
        if (stopArea.isTriggered)
        {
            signalImage.color = new Color(1f, 1f, 1f, brightAlpha);
            signalImage.sprite = redSprite;
        }

        isBlinking = false;
    }
}
