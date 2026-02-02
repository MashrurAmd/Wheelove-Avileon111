using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TriggerZone : MonoBehaviour
{
    [Header("Trigger State")]
    public bool isTriggered = false;

    void Awake()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        box.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isTriggered = true;

        // ✅ ONLY show question if THIS object is a collectible
        if (CompareTag("Collectible"))
        {
            if (QuestionManager.Instance != null)
                QuestionManager.Instance.ShowNextQuestion();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isTriggered = false;
    }
}
