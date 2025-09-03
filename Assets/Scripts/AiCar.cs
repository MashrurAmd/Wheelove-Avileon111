using UnityEngine;

public class AICarController : MonoBehaviour
{
    public float motorForce = 1500f;
    public float brakeForce = 2000f;
    public float maxSteerAngle = 30f;
    public float waypointDistance = 5f;

    private GameObject currentCollectible;

    public WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    public Transform frontLeftWheelTransform, frontRightWheelTransform;
    public Transform rearLeftWheelTransform, rearRightWheelTransform;

    private int currentWaypoint = 0;
    private bool isGasPressed = false;

    [Header("Score System")]
    public int score = 0;

    [Header("UI References")]
    public GameObject questionPanel;

    [Header("Anti-Roll Settings")]
    public float antiRoll = 5000.0f;
    private Rigidbody rb;

    // Gas system
    public static bool isCarMoving = false;
    private GasBar gasBar;

    // ✅ Stop-zone detection
    private bool isInsideStopZone = false;
    private bool hasPrintedStopMessage = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);

        gasBar = FindObjectOfType<GasBar>();
    }

    private void FixedUpdate()
    {
        Steer();

        if (isGasPressed)
        {
            isCarMoving = true;
            ApplyMotorTorque(motorForce);
            ApplyBrakeTorque(0f);
            rb.constraints = RigidbodyConstraints.None;
        }
        else
        {
            isCarMoving = false;
            ApplyMotorTorque(0f);
            ApplyBrakeTorque(brakeForce * 0.2f);
        }

        ApplyAntiRoll(frontLeftWheelCollider, frontRightWheelCollider);
        ApplyAntiRoll(rearLeftWheelCollider, rearRightWheelCollider);

        UpdateWheels();

        // ✅ Check if car has fully stopped inside traffic block (print once)
        if (isInsideStopZone && !isGasPressed && rb.velocity.magnitude < 0.1f && !hasPrintedStopMessage)
        {
            hasPrintedStopMessage = true;
            Debug.Log("🚗 Car has stopped inside traffic block!");
        }
    }

    private void ApplyAntiRoll(WheelCollider wheelL, WheelCollider wheelR)
    {
        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = wheelL.GetGroundHit(out hit);
        if (groundedL)
            travelL = (-wheelL.transform.InverseTransformPoint(hit.point).y - wheelL.radius) / wheelL.suspensionDistance;

        bool groundedR = wheelR.GetGroundHit(out hit);
        if (groundedR)
            travelR = (-wheelR.transform.InverseTransformPoint(hit.point).y - wheelR.radius) / wheelR.suspensionDistance;

        float antiRollForce = (travelL - travelR) * antiRoll;

        if (groundedL)
            rb.AddForceAtPosition(wheelL.transform.up * -antiRollForce, wheelL.transform.position);

        if (groundedR)
            rb.AddForceAtPosition(wheelR.transform.up * antiRollForce, wheelR.transform.position);
    }

    private void Steer()
    {
        if (WaypointManager.waypoints.Count == 0) return;

        Vector3 relativeVector = transform.InverseTransformPoint(
            WaypointManager.waypoints[currentWaypoint].position
        );

        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        frontLeftWheelCollider.steerAngle = newSteer;
        frontRightWheelCollider.steerAngle = newSteer;

        if (relativeVector.magnitude < waypointDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % WaypointManager.waypoints.Count;
        }
    }

    private void ApplyMotorTorque(float torque)
    {
        frontLeftWheelCollider.motorTorque = torque;
        frontRightWheelCollider.motorTorque = torque;
        rearLeftWheelCollider.motorTorque = torque;
        rearRightWheelCollider.motorTorque = torque;
    }

    private void ApplyBrakeTorque(float torque)
    {
        frontLeftWheelCollider.brakeTorque = torque;
        frontRightWheelCollider.brakeTorque = torque;
        rearLeftWheelCollider.brakeTorque = torque;
        rearRightWheelCollider.brakeTorque = torque;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    // UI Button Methods
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
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        
        if (other.CompareTag("traffic"))
        {
            isInsideStopZone = true;
            hasPrintedStopMessage = false; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("traffic"))
        {
            isInsideStopZone = false;
            hasPrintedStopMessage = false;
        }
    }

    // Collectible helper
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

    public void MoveBackWaypoints(int steps)
    {
        currentWaypoint = Mathf.Max(0, currentWaypoint - steps);
        Transform target = WaypointManager.waypoints[currentWaypoint];
        transform.position = target.position;
        transform.rotation = target.rotation;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        ResumeDriving();
    }

    public void RespawnAtStart()
    {
        if (gasBar != null && gasBar.startPoint != null)
        {
            transform.position = gasBar.startPoint.position;
            transform.rotation = gasBar.startPoint.rotation;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            ResumeDriving();
        }
    }
}
