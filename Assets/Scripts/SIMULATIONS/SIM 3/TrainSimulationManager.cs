using UnityEngine;
using System.Collections;

public class TrainSimulationManager : MonoBehaviour
{
    [Header("Trigger Area")]
    public TriggerZoneSpriteHandler triggerZone;

    [Header("Train Settings")]
    public GameObject trainPrefab;

    [Header("Barrier Rod")]
    public Transform barrierRod;

    [Header("Rod Raise Settings")]
    public float waitBeforeRodUp = 20f;
    public float rodTargetZRotation = 50f;
    public float rodRaiseDuration = 1.5f;

    [Header("Player Car")]
    public Car car;

    // internal state
    private bool carInside = false;
    private bool rodRaised = false;

    private Coroutine rodRoutine;

    // saved initial states
    private Quaternion rodInitialRotation;
    private Vector3 trainStartPosition;
    private Quaternion trainStartRotation;

    void Start()
    {
        // save rod initial rotation
        if (barrierRod != null)
            rodInitialRotation = barrierRod.localRotation;

        // save train initial transform
        if (trainPrefab != null)
        {
            trainStartPosition = trainPrefab.transform.position;
            trainStartRotation = trainPrefab.transform.rotation;
            trainPrefab.SetActive(false);
        }
    }

    void Update()
    {
        if (triggerZone == null) return;

        bool isTriggered = triggerZone.GetComponent<TriggerZone>().isTriggered;

        if (isTriggered && !carInside)
        {
            OnCarEntered();
        }
        else if (!isTriggered && carInside)
        {
            OnCarExited();
        }
    }

    // 🚗 CAR ENTERS ZONE
    void OnCarEntered()
    {
        carInside = true;
        rodRaised = false;

        Debug.Log("🚗 Car entered train crossing");

        // enable train
        if (trainPrefab != null)
            trainPrefab.SetActive(true);

        // start barrier timer
        if (rodRoutine != null)
            StopCoroutine(rodRoutine);

        rodRoutine = StartCoroutine(RaiseRodAfterDelay());
    }

    // 🚗 CAR EXITS ZONE
    void OnCarExited()
    {
        carInside = false;

        if (!rodRaised)
        {
            Debug.Log("❌ Rule broken: Car crossed before barrier opened");

            car.PauseCar();
            car.MoveBackByWaypoints(3);
            car.ResumeDriving();
        }
        else
        {
            Debug.Log("✅ Rule followed: Car waited for barrier");
        }

        ResetSimulation();
    }

    // 🚧 BARRIER TIMER
    IEnumerator RaiseRodAfterDelay()
    {
        yield return new WaitForSeconds(waitBeforeRodUp);

        if (!carInside)
            yield break;

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

    // 🔄 FULL RESET
    void ResetSimulation()
    {
        // stop coroutine
        if (rodRoutine != null)
        {
            StopCoroutine(rodRoutine);
            rodRoutine = null;
        }

        carInside = false;
        rodRaised = false;

        // reset barrier
        if (barrierRod != null)
            barrierRod.localRotation = rodInitialRotation;

        // reset train
        if (trainPrefab != null)
        {
            trainPrefab.SetActive(false);
            trainPrefab.transform.position = trainStartPosition;
            trainPrefab.transform.rotation = trainStartRotation;
        }

        Debug.Log("🔄 Train simulation reset");
    }
}
