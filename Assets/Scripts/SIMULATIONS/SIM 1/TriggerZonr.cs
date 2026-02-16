using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class TriggerZone : MonoBehaviour
{
    [Header("Trigger State")]
    public bool isTriggered = false;   // ← RESTORED for other scripts

    private bool hasTriggered = false; // internal safety

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
        if (!other.CompareTag("Player"))
            return;

        if (hasTriggered)
            return;

        hasTriggered = true;
        isTriggered = true;   // ← keep compatibility

        // ===============================
        // 🎯 COLLECTIBLE
        // ===============================
        if (CompareTag("Collectible"))
        {
            Car car = other.GetComponent<Car>();
            if (car != null)
                car.PauseCar();

            QuestionManager qm = FindObjectOfType<QuestionManager>();

            if (qm != null && qm.isActiveAndEnabled)
            {
                qm.ShowNextQuestion();
            }
        }
        // ===============================
        // 🏁 FINISH
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

        isTriggered = false;
    }

    private IEnumerator ShowPanelAndLoadScene()
    {
        if (congratulationsPanel != null)
            congratulationsPanel.SetActive(true);

        yield return new WaitForSeconds(panelDelay);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
