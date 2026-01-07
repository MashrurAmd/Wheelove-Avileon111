using System.Collections.Generic;
using UnityEngine;

public class StopSimulationController : MonoBehaviour
{
    [Header("References")]
    public Car car;                        // your car script
    public TriggerZone stopArea;           // stop zone trigger
    public GasButtonBlink gasButtonBlink;  // 🌟 actual gas button blink script

    [Header("Stop Settings")]
    public float stopThreshold = 0.05f;    // when car is considered stopped

    private bool hasStoppedInside = false;
    private bool wasInside = false;

    void Start()
    {
        // 🔗 bind gas button blink to this stop area
        if (gasButtonBlink != null)
        {
            gasButtonBlink.triggerZones = new List<TriggerZone> { stopArea }; // or multiple stop areas
        }

    }

    void Update()
    {
        if (stopArea == null || car == null)
            return;

        // 🚗 car is inside stop zone
        if (stopArea.isTriggered)
        {
            wasInside = true;

            // check if car fully stopped
            if (car.CurrentSpeed <= stopThreshold)
            {
                hasStoppedInside = true;
            }
        }
        else
        {
            // 🧠 when car exits → evaluate result ONCE
            if (wasInside)
            {
                if (hasStoppedInside)
                    Debug.Log("Successfully stopped at the stop sign 👍");
                else
                    Debug.Log("Rule broken — car did NOT stop ❌");
            }

            // reset for next attempt
            wasInside = false;
            hasStoppedInside = false;
        }
    }
}
