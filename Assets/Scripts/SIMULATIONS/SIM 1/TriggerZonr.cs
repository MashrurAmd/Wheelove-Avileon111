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
    public int maxTriggerCount = 2; // ← how many questions this collectible will show
    private int triggerCount = 0;   // ← how many times it has been triggered

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

        hasTriggered = true;
        isTriggered = true;

        // ===============================
        // 🎯 COLLECTIBLE
        // ===============================
        if (CompareTag("Collectible"))
        {
            // ← Stop if this collectible has been used maxTriggerCount times
            if (triggerCount >= maxTriggerCount)
                return;

            triggerCount++;

            Car car = other.GetComponent<Car>();
            if (car != null)
                car.PauseCar();

            QuestionManager qm = FindObjectOfType<QuestionManager>();
            if (qm != null && qm.isActiveAndEnabled)
                qm.ShowNextQuestion();
        }
        // ===============================
        // 🏁 FINISH LINE
        // ===============================

        //FOR TESTING - ALLOW FINISH WITHOUT QUESTIONS
        //else if (CompareTag("Finish"))
        //{
        //    StartCoroutine(ShowPanelAndLoadScene());
        //}

        else if (CompareTag("Finish"))
        {
            // ← Only load next scene if all questions are answered
            QuestionManager qm = FindObjectOfType<QuestionManager>();
            if (qm != null && !qm.AllQuestionsAnswered())
                return; // ← block finish until all questions done

            StartCoroutine(ShowPanelAndLoadScene());
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (CompareTag("Collectible"))
        {
            // ← Only reset if still has triggers remaining
            if (triggerCount < maxTriggerCount)
            {
                hasTriggered = false;
                isTriggered = false;
            }
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