using UnityEngine;

public class TrafficRuleChecker : MonoBehaviour
{
    public TrafficLightController trafficLight;
    public TriggerZone carTriggerZone;

    private bool carWasInsideOnRed = false;
    private bool carStoppedProperly = false;

    public Car car;



    void Update()
    {
        // ✔️ Car enters during RED
        if (carTriggerZone.isTriggered && trafficLight.IsRedLight())
        {
            carWasInsideOnRed = true;
        }

        // 🟢 When it turns GREEN while car is inside → car obeyed rule
        if (carWasInsideOnRed && carTriggerZone.isTriggered && !trafficLight.IsRedLight())
        {
            carStoppedProperly = true;
        }

        // 🚗 Car LEAVES the area
        if (!carTriggerZone.isTriggered && carWasInsideOnRed)
        {
            // ❌ If it never waited for green
            if (!carStoppedProperly)
            {
                Debug.Log("Rule broken: Car crossed during red light");

                car.PauseCar();
                car.MoveBackByWaypoints(3);
                car.ResumeDriving();
            }
            else
            {
                Debug.Log("Successfully crossed the traffic light");
            }

            // reset state
            carWasInsideOnRed = false;
            carStoppedProperly = false;
        }

        // 🚗 Car leaves during GREEN but never saw RED → clean crossing
        if (!carTriggerZone.isTriggered && !carWasInsideOnRed && !trafficLight.IsRedLight())
        {
            Debug.Log("Successfully crossed the traffic light");
        }
    }
}
