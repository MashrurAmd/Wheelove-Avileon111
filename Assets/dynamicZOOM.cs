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

    [Tooltip("Higher = only switches on very sharp curves. Try 20-45")]
    public float curveThreshold = 30f;

    [Header("Manual Override")]
    public bool manualOverride = false;  // ← when true, auto switching stops

    private bool isFirstPerson = true;  // ← tracks current manual state

    //void Update()
    //{
    //    if (manualOverride) return;  // ← skip auto switching if manual

    //    if (path == null || car == null) return;

    //    float nearest = path.FindClosestPoint(car.position, 0, -1, 10);
    //    Vector3 forward1 = path.EvaluateTangent(nearest);
    //    Vector3 forward2 = path.EvaluateTangent(nearest + checkDistance);
    //    float angle = Vector3.Angle(forward1, forward2);

    //    if (angle > curveThreshold)
    //        SetThirdPerson();
    //    else
    //        SetFirstPerson();
    //}

    // ← Assign this to your button onClick
    public void ToggleCamera()
    {
        manualOverride = true;  // stop auto switching when user manually toggles

        if (isFirstPerson)
            SetThirdPerson();
        else
            SetFirstPerson();
    }

    // ← Call this to re-enable auto switching
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