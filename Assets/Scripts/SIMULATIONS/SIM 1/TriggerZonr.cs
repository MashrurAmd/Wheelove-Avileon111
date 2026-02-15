using UnityEngine;
using UnityEngine.SceneManagement; // Needed for scene loading
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class TriggerZone : MonoBehaviour
{
    [Header("Trigger State")]
    public bool isTriggered = false;

    [Header("Congratulations & Scene")]
    public GameObject congratulationsPanel; // Assign your panel in Inspector
    public string nextSceneName;           // Name of the scene to load
    public float panelDelay = 3f;          // Delay before scene loads

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

        // ✅ New feature: Show congratulations panel and load scene
        if (CompareTag("Finish")) // Add this tag to finish zones
        {
            StartCoroutine(ShowPanelAndLoadScene());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isTriggered = false;
    }

    private IEnumerator ShowPanelAndLoadScene()
    {
        if (congratulationsPanel != null)
            congratulationsPanel.SetActive(true);

        yield return new WaitForSeconds(panelDelay);

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
