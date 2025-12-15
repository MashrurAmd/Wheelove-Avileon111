using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleForwardMove : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Move forward constantly
        Vector3 move = transform.forward * moveSpeed;

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        // Apply movement
        controller.Move((move + velocity) * Time.deltaTime);
    }
}
