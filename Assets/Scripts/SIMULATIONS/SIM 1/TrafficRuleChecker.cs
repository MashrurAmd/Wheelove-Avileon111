using UnityEngine;

public class TrafficRuleChecker : MonoBehaviour
{
    public TrafficLightController trafficLight;
    public TriggerZone carTriggerZone;
    public Car car;

    private bool enteredOnRed = false;
    private bool greenReached = false;

    void Update()
    {
        // 🚗 Car enters zone during RED
        if (carTriggerZone.isTriggered && trafficLight.isRed && !enteredOnRed)
        {
            enteredOnRed = true;
            greenReached = false;
        }

        // 🟢 Light turns GREEN while car is waiting inside
        if (enteredOnRed && carTriggerZone.isTriggered && trafficLight.isGreen)
        {
            greenReached = true;
        }

        // 🚗 Car exits zone
        if (!carTriggerZone.isTriggered && enteredOnRed)
        {
            // ✅ VALID crossing: waited for green AND exited on green
            if (greenReached && trafficLight.isGreen)
            {
                // success → do nothing
            }
            else
            {
                // ❌ INVALID crossing
                car.PauseCar();
                car.MoveBackByWaypoints(3);
                car.ResumeDriving();
            }

            // reset state
            enteredOnRed = false;
            greenReached = false;
        }
    }
}
