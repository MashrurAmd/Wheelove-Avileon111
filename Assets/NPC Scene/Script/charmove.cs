using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleForwardMove : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float gravity = -9.81f;
    public float turnInterval = 5f;

    [HideInInspector] public bool canMove = false; // 🔑 controlled by manager

    private CharacterController controller;
    private Vector3 velocity;
    private float timer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!canMove) return; // ❌ stop movement when not allowed

        Vector3 move = transform.forward * moveSpeed;

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move((move + velocity) * Time.deltaTime);

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
