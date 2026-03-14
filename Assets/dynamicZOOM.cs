using UnityEngine;
using Cinemachine;

public class CameraSwapper : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera thirdPersonCam;
    public CinemachineVirtualCamera firstPersonCam;

    [Header("Curve Detection")]
    public CinemachineSmoothPath path;
    public Transform car;
    public float checkDistance = 2f;
    public float curveThreshold = 30f;

    [Header("Camera Offset On Turns")]
    public float turnOffsetAmount = 1.5f;      // ← how much to shift sideways
    public float offsetSmoothSpeed = 3f;       // ← how smooth the shift is

    [Header("Manual Override")]
    public bool manualOverride = false;
    private bool isFirstPerson = true;

    private CinemachineTransposer thirdPersonTransposer;
    private Vector3 defaultOffset;

    void Start()
    {
        // ← Get the transposer from the 3rd person cam to modify its offset
        thirdPersonTransposer = thirdPersonCam.GetCinemachineComponent<CinemachineTransposer>();

        if (thirdPersonTransposer != null)
            defaultOffset = thirdPersonTransposer.m_FollowOffset;
    }

    void Update()
    {
        if (path == null || car == null) return;

        float nearest = path.FindClosestPoint(car.position, 0, -1, 10);
        Vector3 forward1 = path.EvaluateTangent(nearest);
        Vector3 forward2 = path.EvaluateTangent(nearest + checkDistance);
        float angle = Vector3.Angle(forward1, forward2);

        // ← Detect turn direction using cross product
        Vector3 cross = Vector3.Cross(forward1.normalized, forward2.normalized);
        float turnDirection = cross.y; // positive = left turn, negative = right turn

        if (!manualOverride)
        {
            if (angle > curveThreshold)
                SetThirdPerson();
            else
                SetFirstPerson();
        }

        if (!isFirstPerson && thirdPersonTransposer != null)
        {
            float targetX = 0f;

            if (angle > curveThreshold)
            {
                if (turnDirection > 0.01f)
                    targetX = turnOffsetAmount;    // left turn → shift right
                else if (turnDirection < -0.01f)
                    targetX = -turnOffsetAmount;   // right turn → shift left
            }

            Vector3 targetOffset = new Vector3(
                defaultOffset.x + targetX,
                defaultOffset.y,
                defaultOffset.z
            );

            thirdPersonTransposer.m_FollowOffset = Vector3.Lerp(
                thirdPersonTransposer.m_FollowOffset,
                targetOffset,
                Time.deltaTime * offsetSmoothSpeed
            );
        }
        else if (isFirstPerson && thirdPersonTransposer != null)
        {
            thirdPersonTransposer.m_FollowOffset = Vector3.Lerp(
                thirdPersonTransposer.m_FollowOffset,
                defaultOffset,
                Time.deltaTime * offsetSmoothSpeed
            );
        }
    }

    public void ToggleCamera()
    {
        manualOverride = true;
        if (isFirstPerson)
            SetThirdPerson();
        else
            SetFirstPerson();
    }

    public void EnableAutoSwitch()
    {
        manualOverride = false;
    }

    void SetFirstPerson()
    {
        isFirstPerson = true;
        firstPersonCam.Priority = 20;
        thirdPersonCam.Priority = 10;
    }

    void SetThirdPerson()
    {
        isFirstPerson = false;
        thirdPersonCam.Priority = 20;
        firstPersonCam.Priority = 10;
    }
}