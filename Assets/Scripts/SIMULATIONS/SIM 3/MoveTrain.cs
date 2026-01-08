using UnityEngine;

public class MoveOnX : MonoBehaviour
{
    // Change this in the Inspector
    public float moveSpeed = 2f;

    void Update()
    {
        // Move only in X direction
        transform.position += new Vector3(moveSpeed * Time.deltaTime, 0f, 0f);
    }
}
