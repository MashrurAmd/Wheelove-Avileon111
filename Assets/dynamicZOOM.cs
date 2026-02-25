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
    public float curveThreshold = 5f;

    void Update()
    {
        if (path == null || car == null) return;

        float nearest = path.FindClosestPoint(car.position, 0, -1, 10);
        Vector3 forward1 = path.EvaluateTangent(nearest);
        Vector3 forward2 = path.EvaluateTangent(nearest + checkDistance);
        float angle = Vector3.Angle(forward1, forward2);

        if (angle > curveThreshold)
        {
            SetFirstPerson();
        }
        else
        {
            SetThirdPerson();
        }
    }

    void SetFirstPerson()
    {
        firstPersonCam.Priority = 20;
        thirdPersonCam.Priority = 10;
    }

    void SetThirdPerson()
    {
        thirdPersonCam.Priority = 20;
        firstPersonCam.Priority = 10;
    }
}