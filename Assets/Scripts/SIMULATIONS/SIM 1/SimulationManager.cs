using UnityEngine;
using UnityEngine.UI;

public class SimulationManager : MonoBehaviour
{
    [Header("References")]
    public TrafficLightController trafficLight;
    public TriggerZone carTriggerZone;

    [Header("UI")]
    public Image trafficUI;
    public Sprite goSprite;
    public Sprite stopSprite;

    void Update()
    {
        HandleCarUI();
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
