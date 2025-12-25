using UnityEngine;

public class SimpleCarGas : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider rearLeft;
    public WheelCollider rearRight;

    [Header("Movement Settings")]
    public float maxMotorTorque = 1500f;
    public float acceleration = 800f;
    public float deceleration = 1200f;

    private float currentTorque = 0f;
    private bool isGasPressed = false;

    void FixedUpdate()
    {
        // Smooth acceleration & deceleration
        if (isGasPressed)
        {
            currentTorque = Mathf.MoveTowards(
                currentTorque,
                maxMotorTorque,
                acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            currentTorque = Mathf.MoveTowards(
                currentTorque,
                0f,
                deceleration * Time.fixedDeltaTime
            );
        }

        ApplyMotorTorque(currentTorque);
    }

    void ApplyMotorTorque(float torque)
    {
        frontLeft.motorTorque = torque;
        frontRight.motorTorque = torque;
        rearLeft.motorTorque = torque;
        rearRight.motorTorque = torque;
    }

    // --- CALL THESE FROM UI BUTTON OR INPUT ---
    public void GasDown()
    {
        isGasPressed = true;
    }

    public void GasUp()
    {
        isGasPressed = false;
    }
}

