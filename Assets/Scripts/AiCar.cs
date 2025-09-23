using System.Collections;
using UnityEngine;
using Cinemachine;

public class AICarController : MonoBehaviour
{
    [Header("Movement Settings")]
    public CinemachinePathBase roadPath;   // Drag your RoadPath here
    public float moveSpeed = 10f;          // forward speed
    public float deceleration = 5f;        // slows down when gas is released

    private float pathPosition = 0f;       // current distance along path
    private float currentSpeed = 0f;

    [Header("Score System")]
    public int score = 0;

    [Header("UI References")]
    public GameObject questionPanel;

    private GameObject currentCollectible;
    private Rigidbody rb;

    // Gas system
    public static bool isCarMoving = false;
    private GasBar gasBar;

    // Stop-zone detection
    private bool isInsideStopZone = false;
    private bool hasPrintedStopMessage = false;

    // Store coroutine so we can cancel if needed
    private Coroutine greenLightCoroutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // car is fully path-controlled now
        gasBar = FindObjectOfType<GasBar>();
    }

    private void Update()
    {
        // Gas input (from button or touch events)
        if (isGasPressed)
            currentSpeed = moveSpeed;
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);

        // Move along the path
        pathPosition += currentSpeed * Time.deltaTime;
        pathPosition = Mathf.Clamp(pathPosition, 0f, roadPath.PathLength);

        // Update car transform
        transform.position = roadPath.EvaluatePositionAtUnit(pathPosition, CinemachinePathBase.PositionUnits.Distance);
        transform.rotation = roadPath.EvaluateOrientationAtUnit(pathPosition, CinemachinePathBase.PositionUnits.Distance);

        // Stop detection in traffic zone
        if (isInsideStopZone && !isGasPressed && currentSpeed <= 0.01f && !hasPrintedStopMessage)
        {
            hasPrintedStopMessage = true;
            Debug.Log("Car has stopped inside traffic block!");
        }
    }

    // =============================
    //  Collectibles + Traffic Logic
    // =============================

    private bool isGasPressed = false;
    public void GasPressed() => isGasPressed = true;
    public void GasReleased() => isGasPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            if (questionPanel != null && questionPanel.activeSelf) return;

            score++;
            currentCollectible = other.gameObject;

            QuestionManager.Instance.answerText.text = string.Empty;

            if (QuestionManager.Instance != null)
            {
                QuestionManager.Instance.ShowNextQuestion();
                isGasPressed = false;
            }
        }

        if (other.CompareTag("traffic"))
        {
            isInsideStopZone = true;
            hasPrintedStopMessage = false;

            GamePlayManager.instance.trafficlight.GetComponent<MeshRenderer>().material = GamePlayManager.instance.trafficAlertMat;

            if (greenLightCoroutine != null)
                StopCoroutine(greenLightCoroutine);

            greenLightCoroutine = StartCoroutine(ChangeTrafficLightToGreenAfterDelay(5f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("traffic"))
        {
            isInsideStopZone = false;
            hasPrintedStopMessage = false;

            if (greenLightCoroutine != null)
            {
                StopCoroutine(greenLightCoroutine);
                greenLightCoroutine = null;
            }

            GamePlayManager.instance.trafficlight.GetComponent<MeshRenderer>().material = GamePlayManager.instance.trafficAlertMat;
        }
    }

    private IEnumerator ChangeTrafficLightToGreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GamePlayManager.instance.trafficlight.GetComponent<MeshRenderer>().material =
            GamePlayManager.instance.trafficNormalMat;

        greenLightCoroutine = null;
    }

    public void DismissCollectible()
    {
        if (currentCollectible != null)
        {
            currentCollectible.SetActive(false);
            currentCollectible = null;
        }
    }

    public void ResumeDriving()
    {
        if (questionPanel != null)
        {
            //questionPanel.SetActive(false);
        }
    }

    public void RespawnAtStart()
    {
        if (gasBar != null && gasBar.startPoint != null)
        {
            pathPosition = 0f; // reset to beginning of road
            transform.position = roadPath.EvaluatePositionAtUnit(0, CinemachinePathBase.PositionUnits.Distance);
            transform.rotation = roadPath.EvaluateOrientationAtUnit(0, CinemachinePathBase.PositionUnits.Distance);
        }
    }
}
