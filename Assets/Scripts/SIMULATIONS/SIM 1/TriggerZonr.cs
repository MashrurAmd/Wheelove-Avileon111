using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class TriggerZone : MonoBehaviour
{
    [Header("Trigger State")]
    public bool isTriggered = false;

    private bool hasTriggered = false;

    [Header("Congratulations & Scene")]
    public GameObject congratulationsPanel;
    public string nextSceneName;
    public float panelDelay = 3f;

    private void Awake()
    {
        // Ensure the BoxCollider is set to trigger
        BoxCollider box = GetComponent<BoxCollider>();
        box.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (hasTriggered)
            return;

        hasTriggered = true;
        isTriggered = true;

        // ===============================
        // 🎯 COLLECTIBLE
        // ===============================
        if (CompareTag("Collectible"))
        {
            Car car = other.GetComponent<Car>();
            if (car != null)
                car.PauseCar(); // Stop car until player answers

            QuestionManager qm = FindObjectOfType<QuestionManager>();
            if (qm != null && qm.isActiveAndEnabled)
                qm.ShowNextQuestion();
        }
        // ===============================
        // 🏁 FINISH LINE
        // ===============================
        else if (CompareTag("Finish"))
        {
            StartCoroutine(ShowPanelAndLoadScene());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Reset collectible trigger so it can be triggered again next time
        if (CompareTag("Collectible"))
        {
            hasTriggered = false;
            isTriggered = false;
        }
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
