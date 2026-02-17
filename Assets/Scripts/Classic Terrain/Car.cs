using UnityEngine;
using Cinemachine;

public class Car : MonoBehaviour
{
    [Header("Movement Settings")]
    public CinemachinePathBase roadPath;

    [Tooltip("Max forward speed in m/s along the path")]
    public float maxSpeed = 15f;
    public float acceleration = 5f;
    public float deceleration = 5f;

    private float pathPosition = 0f;

    [SerializeField]
    private float currentSpeed = 0f;

    private Rigidbody rb;
    private bool isGasPressed = false;
    public static bool isCarMoving = false;

    public float CurrentSpeed => currentSpeed;

    [Header("Start Position")]
    public int startWaypointIndex = 0;

    private CinemachineSmoothPath smoothPath;

    [Header("Level 5 Auto Drive")]
    public bool autoDriveLevel5 = false;
    public float autoDriveSpeed = 20f;

    private bool isForcedStopped = false;

    // =========================
    // START
    // =========================

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        smoothPath = roadPath as CinemachineSmoothPath;

        if (smoothPath == null)
        {
            Debug.LogError("Road path must be CinemachineSmoothPath.");
            return;
        }

        startWaypointIndex = Mathf.Clamp(
            startWaypointIndex,
            0,
            smoothPath.m_Waypoints.Length - 1
        );

        SpawnAtWaypoint(startWaypointIndex);
    }

    // =========================
    // OPTIMIZED UPDATE
    // =========================

    private void LateUpdate()
    {
        float targetSpeed = 0f;

        // LEVEL 5 AUTO DRIVE
        if (autoDriveLevel5 && !isForcedStopped)
        {
            targetSpeed = autoDriveSpeed;
        }
        else if (!autoDriveLevel5)
        {
            // NORMAL CONTROL
            targetSpeed = isGasPressed ? maxSpeed : 0f;
        }

        // MOVE SPEED TOWARDS TARGET
        float rate = (targetSpeed > currentSpeed) ? acceleration : deceleration;
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            rate * Time.deltaTime
        );

        // MOVING STATE
        isCarMoving = currentSpeed > 0.01f;

        // PATH MOVEMENT
        pathPosition += currentSpeed * Time.deltaTime;
        pathPosition = Mathf.Clamp(pathPosition, 0f, roadPath.PathLength);

        SetCarToPathPosition();
    }

    // =========================
    // CONTROLS
    // =========================

    public void GasPressed() => isGasPressed = true;
    public void GasReleased() => isGasPressed = false;

    public float GetPathPosition() => pathPosition;
    public bool IsGasPressed() => isGasPressed;

    public void ResumeDriving()
    {
        isForcedStopped = false;
        isCarMoving = true;
    }

    public void PauseCar()
    {
        isForcedStopped = true;
        isGasPressed = false;
        currentSpeed = 0f;
        isCarMoving = false;
    }

    public void RespawnAtStart()
    {
        pathPosition = 0f;
        currentSpeed *= 0.2f;
        isGasPressed = false;
        isCarMoving = false;
        SetCarToPathPosition();
    }

    // =========================
    // PUNISHMENTS
    // =========================

    public void MoveBackByWaypoints(int count)
    {
        if (smoothPath == null)
            smoothPath = roadPath as CinemachineSmoothPath;

        int total = smoothPath.m_Waypoints.Length;

        int currentIndex = Mathf.RoundToInt(
            (pathPosition / roadPath.PathLength) * (total - 1)
        );

        currentIndex -= count;
        currentIndex = Mathf.Clamp(currentIndex, 0, total - 1);

        float t = (float)currentIndex / (total - 1);
        pathPosition = t * roadPath.PathLength;

        currentSpeed = 0f;
        SetCarToPathPosition();
    }

    public void MoveBackTenWaypoints()
    {
        MoveBackByWaypoints(10);
    }

    // =========================
    // INTERNAL
    // =========================

    private void SetCarToPathPosition()
    {
        transform.position = roadPath.EvaluatePositionAtUnit(
            pathPosition,
            CinemachinePathBase.PositionUnits.Distance
        );

        transform.rotation = roadPath.EvaluateOrientationAtUnit(
            pathPosition,
            CinemachinePathBase.PositionUnits.Distance
        );
    }

    private void SpawnAtWaypoint(int index)
    {
        int total = smoothPath.m_Waypoints.Length;

        index = Mathf.Clamp(index, 0, total - 1);

        float t = (float)index / (total - 1);
        pathPosition = t * roadPath.PathLength;

        SetCarToPathPosition();
    }
}
