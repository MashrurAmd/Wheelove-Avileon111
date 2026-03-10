using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class TriggerZone : MonoBehaviour
{
    [Header("Trigger State")]
    public bool isTriggered = false;
    private bool hasTriggered = false;

    [Header("Collectible Settings")]
    public int maxTriggerCount = 2;
    private int triggerCount = 0;

    [Header("Congratulations & Scene")]
    public GameObject congratulationsPanel;
    public string nextSceneName;
    public float panelDelay = 3f;

    private void Awake()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        box.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasTriggered) return;
        if (triggerCount >= maxTriggerCount) return;

        hasTriggered = true;
        isTriggered = true;

        if (CompareTag("Collectible"))
        {
            Car car = other.GetComponent<Car>();
            if (car != null)
                car.PauseCar();

            QuestionManager qm = FindObjectOfType<QuestionManager>();
            if (qm != null && qm.isActiveAndEnabled)
                qm.ShowNextQuestion(this); // ← pass reference to this zone
        }
        else if (CompareTag("Finish"))
        {
            QuestionManager qm = FindObjectOfType<QuestionManager>();
            if (qm != null && !qm.AllQuestionsAnswered())
                return;

            StartCoroutine(ShowPanelAndLoadScene());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (CompareTag("Collectible"))
        {
            hasTriggered = false;
            isTriggered = false;
        }
    }

    // ← Called by QuestionManager only on correct answer
    public void OnQuestionAnsweredCorrectly()
    {
        triggerCount++;
        Debug.Log($"{gameObject.name} triggerCount: {triggerCount}/{maxTriggerCount}");
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