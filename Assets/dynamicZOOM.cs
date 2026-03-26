using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraSwapper : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera thirdPersonCam;
    public CinemachineVirtualCamera firstPersonCam;

    [Header("References")]
    public CinemachineSmoothPath path;
    public Transform car;
    public Car carController;

    [Header("Path Settings")]
    public float checkDistance = 2f;
    public float curveThreshold = 5f;

    [Header("Default Offset (Straight Road)")]
    public Vector3 straightOffset = new Vector3(0f, 3f, -6f);

    [Header("Turn Camera Settings")]
    public float turnSideShift = 1.5f;
    public float turnPullBack = 1.5f;
    public float turnRaiseHeight = 0.8f;

    [Header("Speed Effects")]
    public float maxSpeedForEffect = 20f;
    public float speedPullBack = 4f;
    public float speedLowering = 0.5f;

    [Header("Smoothing")]
    public float offsetSmoothSpeed = 0.4f;

    [Header("Inertia (Camera Weight)")]
    public float inertiaAmount = 0.3f;

    [Header("Manual Override")]
    public bool manualOverride = false;
    private bool isFirstPerson = false;

    [Header("Intro Zoom")]
    public float introDuration = 2.5f;
    public Vector3 introStartOffset = new Vector3(0f, 15f, -25f);
    public bool playIntroOnStart = true;

    private CinemachineTransposer thirdPersonTransposer;
    private bool introComplete = false;

    void Start()
    {
        thirdPersonTransposer = thirdPersonCam.GetCinemachineComponent<CinemachineTransposer>();

        if (thirdPersonTransposer != null)
            thirdPersonTransposer.m_FollowOffset = straightOffset;

        SetThirdPerson();

        if (playIntroOnStart)
            StartCoroutine(PlayIntroZoom());
        else
            introComplete = true;
    }

    void Update()
    {
        if (!introComplete) return;
        if (path == null || car == null) return;
        if (isFirstPerson || thirdPersonTransposer == null) return;

        float speed = carController != null ? carController.CurrentSpeed : 0f;
        float speedFactor = Mathf.Clamp01(speed / maxSpeedForEffect);

        // ------------------------
        // TURN DETECTION (look ahead)
        // ------------------------
        float nearest = path.FindClosestPoint(car.position, 0, -1, 10);

        Vector3 forward1 = path.EvaluateTangent(nearest);
        Vector3 forward2 = path.EvaluateTangent(nearest + checkDistance * 2f);

        float angle = Vector3.Angle(forward1, forward2);
        Vector3 cross = Vector3.Cross(forward1.normalized, forward2.normalized);
        float turnDirection = cross.y;

        // ------------------------
        // BASE OFFSET (speed based)
        // ------------------------
        Vector3 targetOffset = straightOffset;

        // Speed effect (pull back + slight dip)
        targetOffset.z -= speedPullBack * speedFactor;
        targetOffset.y -= speedLowering * speedFactor;

        // ------------------------
        // TURN EFFECT
        // ------------------------
        if (angle > curveThreshold)
        {
            float intensity = Mathf.Clamp01(angle / 25f);

            float sideShift = -turnDirection * turnSideShift * intensity;

            targetOffset += new Vector3(
                sideShift,
                turnRaiseHeight * intensity,
                -turnPullBack * intensity
            );
        }

        // ------------------------
        // SMOOTH TRANSITION
        // ------------------------
        thirdPersonTransposer.m_FollowOffset = Vector3.Lerp(
            thirdPersonTransposer.m_FollowOffset,
            targetOffset,
            Time.deltaTime * (1f / offsetSmoothSpeed)
        );

        // ------------------------
        // INERTIA (camera weight feel)
        // ------------------------
        Vector3 movementDir = (car.position - transform.position).normalized;
        thirdPersonTransposer.m_FollowOffset += movementDir * inertiaAmount * speedFactor * Time.deltaTime;
    }

    // =========================
    // CAMERA SWITCHING
    // =========================

    public void ToggleCamera()
    {
        if (isFirstPerson)
            SetThirdPerson();
        else
            SetFirstPerson();
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

    // =========================
    // INTRO ZOOM
    // =========================

    IEnumerator PlayIntroZoom()
    {
        introComplete = false;

        if (thirdPersonTransposer != null)
            thirdPersonTransposer.m_FollowOffset = introStartOffset;

        float elapsed = 0f;

        while (elapsed < introDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / introDuration;

            // smoothstep
            float smoothT = t * t * (3f - 2f * t);

            if (thirdPersonTransposer != null)
                thirdPersonTransposer.m_FollowOffset = Vector3.Lerp(
                    introStartOffset,
                    straightOffset,
                    smoothT
                );

            yield return null;
        }

        if (thirdPersonTransposer != null)
            thirdPersonTransposer.m_FollowOffset = straightOffset;

        introComplete = true;
    }
}