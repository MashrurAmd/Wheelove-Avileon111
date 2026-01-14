using UnityEngine;

public class CrossRoadManager : MonoBehaviour
{
    public static CrossRoadManager Instance;

    [Header("References")]
    public TriggerZone playerAreaTrigger;
    public Car playerCar;
    public NPCCarMover npcCar1;
    public NPCCarMover npcCar2;

    private bool simulationRunning = false;
    private bool successPrinted = false;

    public Car car;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!simulationRunning)
        {
            // player enters area
            if (playerAreaTrigger.isTriggered)
            {
                StartSimulation();
            }
        }
        else
        {
            CheckForSuccess();
        }
    }

    void StartSimulation()
    {
        simulationRunning = true;
        successPrinted = false;

        // stop player
        //playerCar.PauseCar();

        // start NPC cars crossing
        npcCar1.StartCrossing();
        npcCar2.StartCrossing();
    }

    void CheckForSuccess()
    {
        // if player pressed gas → fail
        if (playerCar.IsGasPressed())
        {
            Fail();
            return;
        }

        // both NPC finished crossing = success
        if (npcCar1.HasFinished() && npcCar2.HasFinished())
        {
            if (!successPrinted)
            {
                Debug.Log("SUCCESS: Player yielded to NPC cars.");
                successPrinted = true;

                // allow player to continue
                playerCar.ResumeDriving();
            }

            simulationRunning = false;
        }
    }

    public void Fail()
    {
        if (successPrinted) return;

        car.PauseCar();                   // pause briefly for effect
        car.MoveBackByWaypoints(3);       // move back
        car.ResumeDriving();

        Debug.Log("FAILED: Collision or player didn't yield.");
        simulationRunning = false;
    }
}
