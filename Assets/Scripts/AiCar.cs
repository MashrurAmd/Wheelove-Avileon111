using System.Collections;
using UnityEngine;
using Cinemachine;

public class AICarController : MonoBehaviour
{
    [Header("Movement Settings")]
    public CinemachinePathBase roadPath;    // Drag your Cinemachine Path here
    public float moveSpeed = 10f;           // Forward speed
    public float deceleration = 5f;         // Slows down when gas is released

    private float pathPosition = 0f;        // Current distance along path
    private float currentSpeed = 0f;        // Current movement speed

    [Header("Score System")]
    public int score = 0;

    private Rigidbody rb;
    private bool isGasPressed = false;
    public static bool isCarMoving = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // fully path-controlled
    }

    private void Update()
    {
        // Gas input
        if (isGasPressed)
        {
            currentSpeed = moveSpeed;
            isCarMoving = true;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
            if (currentSpeed <= 0.01f) isCarMoving = false;
        }

        // Move along path
        pathPosition += currentSpeed * Time.deltaTime;
        pathPosition = Mathf.Clamp(pathPosition, 0f, roadPath.PathLength);

        transform.position = roadPath.EvaluatePositionAtUnit(pathPosition, CinemachinePathBase.PositionUnits.Distance);
        transform.rotation = roadPath.EvaluateOrientationAtUnit(pathPosition, CinemachinePathBase.PositionUnits.Distance);
    }

    // Gas controls
    public void GasPressed() => isGasPressed = true;
    public void GasReleased() => isGasPressed = false;
    public bool IsGasPressed() => isGasPressed;

    // Path info
    public float GetPathPosition() => pathPosition;

    // Teleport helpers
    public void TeleportBackWaypoints(int steps)
    {
        if (WaypointManager.waypoints.Count == 0)
        {
            Debug.LogWarning("No waypoints found!");
            return;
        }

        // Find closest waypoint to car
        int closestIndex = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < WaypointManager.waypoints.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, WaypointManager.waypoints[i].position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestIndex = i;
            }
        }

        // Calculate new waypoint index
        int newIndex = Mathf.Max(closestIndex - steps, 0);

        // Teleport to that waypoint
        transform.position = WaypointManager.waypoints[newIndex].position;
        transform.rotation = WaypointManager.waypoints[newIndex].rotation;

        // Reset path position along Cinemachine Path
        pathPosition = roadPath.FindClosestPoint(transform.position, 0, -1, 10);
        currentSpeed = 0f;
        isGasPressed = false;
        isCarMoving = false;

        Debug.Log($"Car moved back {steps} waypoint(s) to waypoint {newIndex}");
    }

    public void RespawnAtStart()
    {
        pathPosition = 0f;
        transform.position = roadPath.EvaluatePositionAtUnit(0, CinemachinePathBase.PositionUnits.Distance);
        transform.rotation = roadPath.EvaluateOrientationAtUnit(0, CinemachinePathBase.PositionUnits.Distance);

        currentSpeed = 0f;
        isGasPressed = false;
        isCarMoving = false;

        Debug.Log("Car respawned at start.");
    }

    public void PauseCar()
    {
        isGasPressed = false;
        isCarMoving = false;
        currentSpeed = 0f;
    }

    public void ResumeDriving()
    {
        isCarMoving = isGasPressed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            // Stop the car
            PauseCar();

            // Show the question
            if (QuestionManager.Instance != null)
            {
                QuestionManager.Instance.ShowNextQuestion();
            }

            // Disable collectible
            other.gameObject.SetActive(false);
        }
    }
}
