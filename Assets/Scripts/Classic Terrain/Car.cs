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

    private float pathPosition = 0f;  // ALWAYS IN DISTANCE UNITS

    [SerializeField]
    private float currentSpeed = 0f;

    private Rigidbody rb;
    private bool isGasPressed = false;
    public static bool isCarMoving = false;

    public float CurrentSpeed => currentSpeed;

    [Header("Start Position")]
    public float startDistance = 0f;  // distance along path, NOT waypoint index

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // clamp start distance safely
        startDistance = Mathf.Clamp(startDistance, 0f, roadPath.PathLength);

        // set starting position by distance
        pathPosition = startDistance;

        transform.position = roadPath.EvaluatePositionAtUnit(
            pathPosition,
            CinemachinePathBase.PositionUnits.Distance
        );

        transform.rotation = roadPath.EvaluateOrientationAtUnit(
            pathPosition,
            CinemachinePathBase.PositionUnits.Distance
        );
    }

    private void LateUpdate()
    {
        // --- ACCELERATION / BRAKING ---
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

        // clamp safety
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        // --- MOVE ALONG PATH USING REAL DISTANCE ---
        pathPosition += currentSpeed * Time.deltaTime;
        pathPosition = Mathf.Clamp(pathPosition, 0f, roadPath.PathLength);

        transform.position = roadPath.EvaluatePositionAtUnit(
            pathPosition,
            CinemachinePathBase.PositionUnits.Distance
        );

        transform.rotation = roadPath.EvaluateOrientationAtUnit(
            pathPosition,
            CinemachinePathBase.PositionUnits.Distance
        );
    }

    public void GasPressed() => isGasPressed = true;
    public void GasReleased() => isGasPressed = false;

    public float GetPathPosition() => pathPosition;

    public void RespawnAtStart()
    {
        pathPosition = 0f;
        currentSpeed = 0f;
        isGasPressed = false;
        isCarMoving = false;

        transform.position = roadPath.EvaluatePositionAtUnit(
            0,
            CinemachinePathBase.PositionUnits.Distance
        );

        transform.rotation = roadPath.EvaluateOrientationAtUnit(
            0,
            CinemachinePathBase.PositionUnits.Distance
        );
    }

    public bool IsGasPressed()
    {
        return isGasPressed;
    }

    public void ResumeDriving()
    {
        // restores movement based on whether gas is held
        isCarMoving = isGasPressed;
    }

}
