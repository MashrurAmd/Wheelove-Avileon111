using UnityEngine;
using UnityEngine.UI;

public class SimulationManager : MonoBehaviour
{
    [Header("References")]
    public TrafficLightController trafficLight;
    public SimpleForwardMove pedestrian;
    public TriggerZone carTriggerZone;

    [Header("UI")]
    public Image trafficUI;
    public Sprite goSprite;
    public Sprite stopSprite;

    void Update()
    {
        HandlePedestrian();
        HandleCarUI();
    }

    // 🚶 Pedestrian logic
    void HandlePedestrian()
    {
        if (trafficLight.IsRedLight())
        {
            pedestrian.canMove = true;   // Red = people walk
        }
        else
        {
            pedestrian.canMove = false;  // Green = stop
        }
    }

    // 🚗 Car UI logic
    void HandleCarUI()
    {
        if (carTriggerZone.isTriggered)
        {
            trafficUI.sprite = stopSprite; // Car in area = STOP
        }
        else
        {
            trafficUI.sprite = goSprite;   // Car not in area = GO
        }
    }
}
