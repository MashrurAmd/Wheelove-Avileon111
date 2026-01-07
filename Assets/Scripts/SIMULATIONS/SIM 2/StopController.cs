using UnityEngine;
using UnityEngine.UI;

public class StopSimulationController : MonoBehaviour
{
    [Header("References")]
    public Car car;
    public TriggerZone stopArea;
    public Image signalImage;
    public Sprite greenSprite;
    public Sprite redSprite;

    private bool hasStoppedInside = false;
    private bool wasInside = false;

    void Update()
    {
        if (stopArea == null)
        {
            Debug.LogError("Stop area NOT assigned in inspector");
            return;
        }

        if (car == null)
        {
            Debug.LogError("Car NOT assigned in inspector");
            return;
        }

        // 🚗 Car inside stop zone
        if (stopArea.isTriggered)
        {
            signalImage.sprite = redSprite;

            if (!wasInside)
                Debug.Log("Car ENTERED stop area");

            wasInside = true;

            if (car.CurrentSpeed <= 0.05f)
            {
                if (!hasStoppedInside)
                    Debug.Log("Car STOPPED inside the area");

                hasStoppedInside = true;
            }
        }
        else
        {
            signalImage.sprite = greenSprite;

            if (wasInside)
            {
                if (hasStoppedInside)
                    Debug.Log("Successfully stopped at the stop sign 👍");
                else
                    Debug.Log("Rule broken — did NOT stop ❌");
            }

            wasInside = false;
            hasStoppedInside = false;
        }
    }

}
