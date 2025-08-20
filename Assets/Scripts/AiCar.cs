using UnityEngine;

public class AICarController : MonoBehaviour
{
    public float motorForce = 1500f;
    public float brakeForce = 2000f;
    public float maxSteerAngle = 30f;
    public float waypointDistance = 5f;

    public WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    public Transform frontLeftWheelTransform, frontRightWheelTransform;
    public Transform rearLeftWheelTransform, rearRightWheelTransform;

    private int currentWaypoint = 0;
    private bool isGasPressed = false;

    [Header("Score System")]
    public int score = 0;

    [Header("UI References")]
    public GameObject questionPanel; // Assign in Inspector

    [Header("Anti-Roll Settings")]
    public float antiRoll = 5000.0f;
    private Rigidbody rb;

    // 🔹 Gas system
    public static bool isCarMoving = false;
    private GasBar gasBar;

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
            score++;
            other.gameObject.SetActive(false);
            ShowQuestion();
        }
    }

    private void ShowQuestion()
    {
        if (questionPanel != null)
        {
            questionPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // ✅ Correct Answer → add fuel
    public void OnCorrectAnswer()
    {
        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
            Time.timeScale = 1f;

            if (gasBar != null)
                gasBar.AddGas(0.1f);
        }
    }


    // Add these inside AICarController class
    public void ResumeDriving()
    {
        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void MoveBackWaypoints(int steps)
    {
        currentWaypoint = Mathf.Max(0, currentWaypoint - steps);

        // Snap car to that waypoint
        Transform target = WaypointManager.waypoints[currentWaypoint];
        transform.position = target.position;
        transform.rotation = target.rotation;

        // Reset car velocity
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

