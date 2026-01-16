using UnityEngine;

public class SlipperyRoadManager : MonoBehaviour
{
    [Header("References")]
    public TriggerZoneSpriteHandler slipperyZone; // your existing zone
    public Car car;

    [Header("Rules")]
    public float maxAllowedSpeed = 10f;
    public int penaltyWaypoints = 3;

    private bool carInside = false;
    private bool speedWasValid = true;

    void Update()
    {
        if (slipperyZone == null || car == null)
            return;

        bool isTriggered = slipperyZone.GetComponent<Collider>().bounds
            .Contains(car.transform.position);

        // 🚗 ENTER zone
        if (isTriggered && !carInside)
        {
            OnCarEntered();
        }
        // 🚗 EXIT zone
        else if (!isTriggered && carInside)
        {
            OnCarExited();
        }

        // 🚗 While inside → monitor speed
        if (carInside)
        {
            if (car.CurrentSpeed > maxAllowedSpeed)
            {
                speedWasValid = false;
            }
        }
    }

    void OnCarEntered()
    {
        carInside = true;
        speedWasValid = true; // reset check
    }

    void OnCarExited()
    {
        carInside = false;

        if (speedWasValid)
        {
            Debug.Log("✅ Slippery Road Passed Successfully");
        }
        else
        {
            Debug.Log("❌ Slippery Road Failed — Speed too high");

            car.PauseCar();
            car.MoveBackByWaypoints(penaltyWaypoints);
            car.ResumeDriving();
        }
    }
}

