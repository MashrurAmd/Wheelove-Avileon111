using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleForwardMove : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float gravity = -9.81f;
    public float turnInterval = 5f;

    private CharacterController controller;
    private Vector3 velocity;
    private float timer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Move forward
        Vector3 move = transform.forward * moveSpeed;

        // Gravity handling
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        controller.Move((move + velocity) * Time.deltaTime);

        // Timer for rotation
        timer += Time.deltaTime;
        if (timer >= turnInterval)
        {
            Rotate180();
            timer = 0f;
        }
    }

    void Rotate180()
    {
        transform.Rotate(0f, 180f, 0f);
    }
}
