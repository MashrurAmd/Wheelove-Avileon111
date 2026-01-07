using UnityEngine;
using Cinemachine;

public class Car : MonoBehaviour
{
    [Header("Movement Settings")]
    public CinemachinePathBase roadPath;
    public float moveSpeed = 10f;
    public float acceleration = 5f;
    public float deceleration = 5f;

    private float pathPosition = 0f;

    [SerializeField]
    private float currentSpeed = 0f;   // shows in Inspector

    private Rigidbody rb;
    private bool isGasPressed = false;
    public static bool isCarMoving = false;

    // 🔍 PUBLIC SPEED CHECKER (read-only)
    public float CurrentSpeed => currentSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void LateUpdate()
    {
        if (isGasPressed)
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                moveSpeed,
                acceleration * Time.deltaTime
            );
            isCarMoving = true;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                0,
                deceleration * Time.deltaTime
            );

            if (currentSpeed <= 0.01f)
                isCarMoving = false;
        }

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
    public bool IsGasPressed() => isGasPressed;

    public float GetPathPosition() => pathPosition;

    public void RespawnAtStart()
    {
        pathPosition = 0f;

        transform.position = roadPath.EvaluatePositionAtUnit(
            0,
            CinemachinePathBase.PositionUnits.Distance
        );

        transform.rotation = roadPath.EvaluateOrientationAtUnit(
            0,
            CinemachinePathBase.PositionUnits.Distance
        );

        currentSpeed = 0f;
        isGasPressed = false;
        isCarMoving = false;
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
            PauseCar();

            if (QuestionManager.Instance != null)
            {
                QuestionManager.Instance.ShowNextQuestion();
            }

            other.gameObject.SetActive(false);
        }
    }
}
