using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class DynamicCameraZoom : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineDollyCart dollyCart;
    public CinemachineSmoothPath path;

    [Header("Zoom Settings")]
    public float normalZ = -6f;
    public float curveZ = -8f;

    [Header("Look Offset")]
    public float normalX = 0f;
    public float curveX = 1.5f;

    [Header("Sensitivity")]
    public float curveThreshold = 5f;

    private CinemachineTransposer transposer;
    private float lastOffsetZ;
    private float lastOffsetX;

    void Start()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        lastOffsetZ = normalZ;
        lastOffsetX = normalX;
    }

    void Update()
    {
        float t = dollyCart.m_Position / path.PathLength;

        Vector3 forward1 = path.EvaluateTangentAtUnit(t, CinemachinePathBase.PositionUnits.Normalized);
        Vector3 forward2 = path.EvaluateTangentAtUnit(t + 0.01f, CinemachinePathBase.PositionUnits.Normalized);

        float angle = Vector3.Angle(forward1, forward2);

        if (angle > curveThreshold)
        {
            ApplyCurveCamera(forward1, forward2);
        }
        else
        {
            ResetCamera();
        }
    }

    void ApplyCurveCamera(Vector3 f1, Vector3 f2)
    {
        float direction = Mathf.Sign(Vector3.Cross(f1, f2).y);

        if (lastOffsetZ != curveZ)
        {
            lastOffsetZ = curveZ;
            transposer.m_FollowOffset = new Vector3(lastOffsetX, 1.77f, curveZ);


        }

        float targetX = curveX * direction;

        if (lastOffsetX != targetX)
        {
            lastOffsetX = targetX;

            transposer.m_FollowOffset = new Vector3(targetX, 1.77f, curveZ);
        }
    }

    void ResetCamera()
    {
        if (lastOffsetZ != normalZ || lastOffsetX != normalX)
        {
            lastOffsetZ = normalZ;
            lastOffsetX = normalX;

            transposer.m_FollowOffset = new Vector3(normalX, 1.77f, normalZ);
        }
    }
}
