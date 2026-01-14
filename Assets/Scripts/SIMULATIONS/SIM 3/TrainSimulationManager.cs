using UnityEngine;
using System.Collections;

public class TrainSimulationManager : MonoBehaviour
{
    [Header("Trigger Area")]
    public TriggerZoneSpriteHandler triggerZone;   // your area script (same zone used already)

    [Header("Train Settings")]
    public GameObject trainPrefab;                 // train object to enable/disable

    [Header("Barrier Rod")]
    public Transform barrierRod;                   // rotating rod object

    [Header("Rod Raise Settings")]
    public float waitBeforeRodUp = 20f;            // time car must wait
    public float rodTargetZRotation = 50f;         // final Z rotation
    public float rodRaiseDuration = 1.5f;          // how fast rod moves up

    private bool carInside = false;
    private bool rodRaised = false;
    private bool timerStarted = false;

    private Coroutine rodRoutine;
    private Quaternion rodInitialRotation;

    public Car car;

    void Start()
    {
        // store starting rotation
        if (barrierRod != null)
            rodInitialRotation = barrierRod.localRotation;

        // train hidden at start
        if (trainPrefab != null)
            trainPrefab.SetActive(false);
    }

    void Update()
    {
        if (triggerZone == null) return;

        // car inside zone
        if (triggerZone.GetComponent<TriggerZone>().isTriggered)
        {
            if (!carInside)
            {
                OnCarEntered();
            }
        }
        else
        {
            if (carInside)
            {
                OnCarExited();
            }
        }
    }

    void OnCarEntered()
    {
        carInside = true;
        rodRaised = false;
        timerStarted = true;

        Debug.Log("Car entered level crossing area");

        // activate train
        if (trainPrefab != null)
            trainPrefab.SetActive(true);

        // start rod raise timer
        if (rodRoutine != null)
            StopCoroutine(rodRoutine);

        rodRoutine = StartCoroutine(RaiseRodAfterDelay());
    }

    void OnCarExited()
    {
        carInside = false;

        // deactivate train
        if (trainPrefab != null)
            trainPrefab.SetActive(false);

        // evaluate traffic rule
        if (!rodRaised)
        {
            car.PauseCar();
            car.MoveBackByWaypoints(3);
            car.ResumeDriving();

            Debug.Log("❌ Rule broken: car left before barrier opened");
        }
        else
        {
            Debug.Log("✅ Rule followed: car waited until barrier opened");
        }

        // reset rod back down
        if (barrierRod != null)
            barrierRod.localRotation = rodInitialRotation;

        timerStarted = false;
    }

    IEnumerator RaiseRodAfterDelay()
    {
        // wait required time
        yield return new WaitForSeconds(waitBeforeRodUp);

        // if car already left, stop
        if (!carInside)
            yield break;

        // animate rotation to target
        float t = 0f;
        Quaternion start = barrierRod.localRotation;
        Quaternion target = Quaternion.Euler(
            start.eulerAngles.x,
            start.eulerAngles.y,
            rodTargetZRotation
        );

        while (t < 1f)
        {
            t += Time.deltaTime / rodRaiseDuration;
            barrierRod.localRotation = Quaternion.Lerp(start, target, t);
            yield return null;
        }

        rodRaised = true;
        Debug.Log("🚧 Barrier opened — car may pass");
    }
}
