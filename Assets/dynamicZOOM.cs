using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class RacingCameraController : MonoBehaviour
{
    [Header("References")]
    public CinemachineVirtualCamera vcam;
    public CinemachineSmoothPath path;
    public Transform car;

    [Header("Zoom Settings")]
    public float normalZ = -6f;
    public float curveZ = -6f;

    [Header("Side Look")]
    public float sideAmount = 1f;

    [Header("FOV Settings")]
    public float normalFOV = 60f;
    public float curveFOV = 65f;

    [Header("Curve Detection")]
    public float checkDistance = 2f;
    public float curveThreshold = 5f;

    private CinemachineTransposer transposer;

    void Start()
    {
        transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
    }

    void Update()
    {
        if (path == null || car == null) return;

        float nearest = path.FindClosestPoint(car.position, 0, -1, 10);

        Vector3 forward1 = path.EvaluateTangent(nearest);
        Vector3 forward2 = path.EvaluateTangent(nearest + checkDistance);

        float angle = Vector3.Angle(forward1, forward2);

        if (angle > curveThreshold)
        {
            ApplyCurve(forward1, forward2);
        }
        else
        {
            ResetCamera();
        }
    }

    void ApplyCurve(Vector3 f1, Vector3 f2)
    {
        float direction = Mathf.Sign(Vector3.Cross(f1, f2).y);

        Vector3 targetOffset = new Vector3(
            sideAmount * direction,
            1.77f,
            curveZ
        );

        DOTween.To(
            () => transposer.m_FollowOffset,
            x => transposer.m_FollowOffset = x,
            targetOffset,
            0.5f
        ).SetEase(Ease.OutCubic);

        DOTween.To(
            () => vcam.m_Lens.FieldOfView,
            x => vcam.m_Lens.FieldOfView = x,
            curveFOV,
            0.5f
        ).SetEase(Ease.OutCubic);
    }

    void ResetCamera()
    {
        Vector3 targetOffset = new Vector3(
            0f,
            1.77f,
            normalZ
        );

        DOTween.To(
            () => transposer.m_FollowOffset,
            x => transposer.m_FollowOffset = x,
            targetOffset,
            0.5f
        ).SetEase(Ease.OutCubic);

        DOTween.To(
            () => vcam.m_Lens.FieldOfView,
            x => vcam.m_Lens.FieldOfView = x,
            normalFOV,
            0.5f
        ).SetEase(Ease.OutCubic);
    }
}