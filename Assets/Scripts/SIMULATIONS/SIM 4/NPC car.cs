using UnityEngine;

public class NPCCarMover : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    public float moveSpeed = 5f;
    public bool autoMove = false;

    private bool hasMoved = false;
    private bool isMoving = false;

    private void Start()
    {
        // spawn at start
        transform.position = startPoint.position;
    }

    private void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            endPoint.position,
            moveSpeed * Time.deltaTime
        );

        // reached destination
        if (Vector3.Distance(transform.position, endPoint.position) < 0.1f)
        {
            isMoving = false;
            hasMoved = true;
        }
    }

    public void StartCrossing()
    {
        isMoving = true;
        hasMoved = false; // reset so it can move when simulation restarts
    }


    public bool HasFinished()
    {
        return hasMoved;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CrossRoadManager.Instance.Fail();
        }
    }



}
