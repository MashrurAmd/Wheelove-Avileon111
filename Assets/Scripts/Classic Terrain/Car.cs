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

    // ALWAYS distance units along path
    private float pathPosition = 0f;

    [SerializeField]
    private float currentSpeed = 0f;

    private Rigidbody rb;
    private bool isGasPressed = false;
    public static bool isCarMoving = false;

    public float CurrentSpeed => currentSpeed;

    [Header("Start Position")]
    public int startWaypointIndex = 0;

    // cache smooth path ref
    private CinemachineSmoothPath smoothPath;


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

        // clamp to valid waypoint range
        startWaypointIndex = Mathf.Clamp(
            startWaypointIndex,
            0,
            smoothPath.m_Waypoints.Length - 1
        );

        // set car spawn
        SpawnAtWaypoint(startWaypointIndex);
    }


    private void LateUpdate()
    {
        // -------- ACCELERATION / BRAKING --------
        if (isGasPressed)
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                maxSpeed,
                acceleration * Time.deltaTime
            );

            isCarMoving = true;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                0f,
                deceleration * Time.deltaTime
            );

            if (currentSpeed <= 0.01f)
                isCarMoving = false;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        // -------- MOVE ALONG PATH IN DISTANCE UNITS --------
        pathPosition += currentSpeed * Time.deltaTime;
        pathPosition = Mathf.Clamp(pathPosition, 0f, roadPath.PathLength);

        SetCarToPathPosition();
    }


    // ===========================
    //  PUBLIC CONTROL METHODS
    // ===========================

    public void GasPressed() => isGasPressed = true;
    public void GasReleased() => isGasPressed = false;

    public float GetPathPosition() => pathPosition;

    public bool IsGasPressed() => isGasPressed;

    public void ResumeDriving()
    {
        isCarMoving = isGasPressed;
    }

    public void RespawnAtStart()
    {
        pathPosition = 0f;
        currentSpeed = 0f;
        isGasPressed = false;
        isCarMoving = false;

        SetCarToPathPosition();
    }

    // ===========================
    //  WRONG ANSWER PUNISHMENT 🚫
    // ===========================

    /// <summary>
    /// Move the car backwards by N waypoints.
    /// </summary>
    public void MoveBackByWaypoints(int count)
    {
        if (smoothPath == null)
            smoothPath = roadPath as CinemachineSmoothPath;

        int total = smoothPath.m_Waypoints.Length;

        // convert current distance to approx waypoint index
        int currentIndex = Mathf.RoundToInt(
            (pathPosition / roadPath.PathLength) * (total - 1)
        );

        // subtract
        currentIndex -= count;

        // clamp
        currentIndex = Mathf.Clamp(currentIndex, 0, total - 1);

        // convert index back to distance along path
        float t = (float)currentIndex / (total - 1);
        pathPosition = t * roadPath.PathLength;

        // stop movement
        currentSpeed = 0f;

        SetCarToPathPosition();
    }

    /// <summary>
    /// Shortcut: move car back 10 waypoints.
    /// </summary>
    public void MoveBackTenWaypoints()
    {
        MoveBackByWaypoints(10);
    }


    // ===========================
    //  INTERNAL HELPERS
    // ===========================

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


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            isGasPressed = false;
            currentSpeed = 0f;
            isCarMoving = false;

            if (QuestionManager.Instance != null)
                QuestionManager.Instance.ShowNextQuestion();
            else
                Debug.LogError("QuestionManager instance is NULL");
        }
    }
    public void PauseCar()
    {
        isGasPressed = false;
        isCarMoving = false;
        currentSpeed = 0f;
    }

}
