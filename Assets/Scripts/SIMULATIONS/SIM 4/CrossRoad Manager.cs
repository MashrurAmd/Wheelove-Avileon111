using UnityEngine;

public class CrossRoadManager : MonoBehaviour
{
    public static CrossRoadManager Instance;

    [Header("References")]
    public SimulationZone playerAreaZone;  // ← changed from TriggerZone
    public NPCCarMover npcCar1;
    public NPCCarMover npcCar2;

    private bool carsAlreadyStarted = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // Player entered zone → start NPC cars ONCE
        if (!carsAlreadyStarted && playerAreaZone.isPlayerInside)
        {
            StartNPCMovement();
        }

        // Reset when player exits so simulation can repeat if needed
        if (carsAlreadyStarted && playerAreaZone.justExited)
        {
            playerAreaZone.ClearExitFlag();
            carsAlreadyStarted = false;
        }
    }

    void StartNPCMovement()
    {
        carsAlreadyStarted = true;

        if (npcCar1 != null) npcCar1.StartCrossing();
        if (npcCar2 != null) npcCar2.StartCrossing();

        Debug.Log("NPC cars are crossing now");
    }

    public void Fail()
    {
        Debug.Log("FAILED: Player hit NPC car");
    }
}