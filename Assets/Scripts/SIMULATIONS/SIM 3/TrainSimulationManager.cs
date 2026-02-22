using UnityEngine;
using System.Collections;

public class TrainSimulationManager : MonoBehaviour
{
    [Header("Trigger Area")]
    public SimulationZone triggerZone;      // ← changed from TriggerZone

    [Header("Train Settings")]
    public GameObject train;

    [Header("Barrier Rod")]
    public Transform barrierRod;

    [Header("Rod Raise Settings")]
    public float waitBeforeRodUp = 20f;
    public float rodTargetZRotation = 50f;
    public float rodRaiseDuration = 1.5f;

    [Header("Reset Settings")]
    public float delayBeforeReset = 3f;

    [Header("Player Car")]
    public Car car;

    private bool carInside = false;
    private bool rodRaised = false;

    private Coroutine rodRoutine;
    private Coroutine resetRoutine;

    private Quaternion rodInitialRotation;
    private Vector3 trainStartPosition;
    private Quaternion trainStartRotation;

    void Start()
    {
        if (barrierRod != null)
            rodInitialRotation = barrierRod.localRotation;

        if (train != null)
        {
            trainStartPosition = train.transform.position;
            trainStartRotation = train.transform.rotation;
            train.SetActive(false);
        }
    }

    void Update()
    {
        if (triggerZone == null) return;

        // Car entered
        if (triggerZone.isPlayerInside && !carInside)
            OnCarEntered();

        // Car exited — use justExited flag
        if (triggerZone.justExited && carInside)
        {
            triggerZone.ClearExitFlag();
            OnCarExited();
        }
    }

    void OnCarEntered()
    {
        carInside = true;
        rodRaised = false;

        Debug.Log("🚗 Car entered train crossing");

        // Activate train
        if (train != null)
        {
            train.transform.position = trainStartPosition;
            train.transform.rotation = trainStartRotation;
            train.SetActive(true);
        }

        // Stop any pending reset
        if (resetRoutine != null)
        {
            StopCoroutine(resetRoutine);
            resetRoutine = null;
        }

        // Start barrier raise timer
        if (rodRoutine != null)
            StopCoroutine(rodRoutine);

        rodRoutine = StartCoroutine(RaiseRodAfterDelay());
    }

    void OnCarExited()
    {
        carInside = false;

        if (!rodRaised)
        {
            Debug.Log("❌ Rule broken: Car crossed before barrier opened");
            car.MoveBackByWaypoints(3);
        }
        else
        {
            Debug.Log("✅ Rule followed: Car waited for barrier");
        }

        if (resetRoutine != null)
            StopCoroutine(resetRoutine);

        resetRoutine = StartCoroutine(ResetAfterDelay());
    }

    IEnumerator RaiseRodAfterDelay()
    {
        // Wait for train to pass
        yield return new WaitForSeconds(waitBeforeRodUp);

        if (!carInside) yield break;

        // Animate barrier rod raising
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
        Debug.Log("🚧 Barrier raised — car may pass ✅");
    }

    IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeReset);
        ResetSimulation();
    }

    void ResetSimulation()
    {
        if (rodRoutine != null)
        {
            StopCoroutine(rodRoutine);
            rodRoutine = null;
        }

        carInside = false;
        rodRaised = false;

        // Reset barrier
        if (barrierRod != null)
            barrierRod.localRotation = rodInitialRotation;

        // Reset train
        if (train != null)
        {
            train.SetActive(false);
            train.transform.position = trainStartPosition;
            train.transform.rotation = trainStartRotation;
        }

        Debug.Log("🔄 Train simulation reset");
    }
}