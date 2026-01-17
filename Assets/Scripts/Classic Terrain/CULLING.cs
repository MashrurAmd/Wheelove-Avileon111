using UnityEngine;

public class DistanceCulling : MonoBehaviour
{
    public float cullDistance = 80f;
    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        float d = Vector3.Distance(transform.position, cam.position);
        gameObject.SetActive(d < cullDistance);
    }
}
