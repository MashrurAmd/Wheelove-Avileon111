using UnityEngine;

public class CrossRoadManager : MonoBehaviour
{
    public static CrossRoadManager Instance;

    [Header("References")]
    public TriggerZone playerAreaTrigger;
    public NPCCarMover npcCar1;
    public NPCCarMover npcCar2;

    private bool carsAlreadyStarted = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // player entered trigger zone
        if (!carsAlreadyStarted && playerAreaTrigger.isTriggered)
        {
            StartNPCMovement();
        }
    }

    void StartNPCMovement()
    {
        carsAlreadyStarted = true;

        if (npcCar1 != null)
            npcCar1.StartCrossing();

        if (npcCar2 != null)
            npcCar2.StartCrossing();

        Debug.Log("NPC cars are crossing now");
    }

    // keep your fail for collision
    public void Fail()
    {
        Debug.Log("FAILED: Player hit NPC car");
    }
}
