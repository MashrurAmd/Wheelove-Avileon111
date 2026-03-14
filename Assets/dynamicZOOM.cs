using System.Collections;
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
    public float turnOffsetAmount = 1.5f;
    public float turnOffsetY = 1f;
    public float turnOffsetZ = 0f;         // ← add this
    public float offsetSmoothSpeed = 3f;

    [Header("Manual Override")]
    public bool manualOverride = false;
    private bool isFirstPerson = true;

    private CinemachineTransposer thirdPersonTransposer;
    private Vector3 defaultOffset;

    [Header("Intro Zoom")]
    public float introDuration = 2.5f;        // ← how long the zoom takes
    public Vector3 introStartOffset = new Vector3(0, 15, -25); // ← far away start position
    public bool playIntroOnStart = true;

    private bool introComplete = false;

    void Start()
    {
        thirdPersonTransposer = thirdPersonCam.GetCinemachineComponent<CinemachineTransposer>();

        if (thirdPersonTransposer != null)
            defaultOffset = thirdPersonTransposer.m_FollowOffset;

        SetThirdPerson();

        if (playIntroOnStart)
            StartCoroutine(PlayIntroZoom());
        else
            introComplete = true;
    }

    void Update()
    {
        //Debug.DrawLine(car.position, car.position + car.forward * 5, Color.red);

        if (!introComplete) return; // ← wait for intro to finish

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
                defaultOffset.y + (angle > curveThreshold ? turnOffsetY : 0f),
                defaultOffset.z + (angle > curveThreshold ? turnOffsetZ : 0f)  // ← add Z
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


    IEnumerator PlayIntroZoom()
    {
        introComplete = false;

        // ← Set camera to far start position
        if (thirdPersonTransposer != null)
            thirdPersonTransposer.m_FollowOffset = introStartOffset;

        float elapsed = 0f;

        while (elapsed < introDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / introDuration;

            // ← Smooth ease in
            float smoothT = t * t * (3f - 2f * t);

            if (thirdPersonTransposer != null)
                thirdPersonTransposer.m_FollowOffset = Vector3.Lerp(
                    introStartOffset,
                    defaultOffset,
                    smoothT
                );

            yield return null;
        }

        if (thirdPersonTransposer != null)
            thirdPersonTransposer.m_FollowOffset = defaultOffset;

        introComplete = true;
    }

}