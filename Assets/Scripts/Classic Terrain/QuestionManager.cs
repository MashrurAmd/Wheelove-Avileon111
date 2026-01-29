using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;

    [Header("Data")]
    public QuesData quesData;

    [Header("UI References")]
    public GameObject questionPanel;
    public TMP_Text questionText;
    public List<Toggle> optionToggles;
    public List<Text> optionLabels;
    public ToggleGroup toggleGroup;
    public TMP_Text timerText;
    public TMP_Text answerText;
    public Text Scoretext;
    public Text wrongAnswersText;
    public GameObject Gameover;

    [Header("Timer")]
    public float questionTime = 10f;

    private int currentTestIndex = 0;
    private int currentQuestionIndex = 0;
    private float currentTime;
    private bool isCountingDown = false;

    private SoundManager soundManager;

    [Header("Game Data")]
    public int score = 0;
    public int life = 3;

    private Car car;
    private GasBar gasBar;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        if (quesData == null || quesData.tests.Count == 0)
        {
            Debug.LogError("❌ QuesData is missing or empty!");
            return;
        }

        car = FindObjectOfType<Car>();
        gasBar = FindObjectOfType<GasBar>();
        soundManager = FindObjectOfType<SoundManager>();

        UpdateScoreUI();
        UpdateWrongAnswersUI();

        if (soundManager != null)
            soundManager.PlayGameplayMusic();

        // ❌ DO NOT show question here
    }


    void Update()
    {
        if (!isCountingDown) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isCountingDown = false;
            CheckAnswer();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
    }

    // ==============================
    // QUESTION FLOW
    // ==============================

    public void ShowNextQuestion()
    {
        if (currentTestIndex >= quesData.tests.Count)
        {
            if (Gameover != null) Gameover.SetActive(true);
            return;
        }

        var test = quesData.tests[currentTestIndex];

        if (currentQuestionIndex >= test.quesAnswers.Count)
        {
            StartNextTest();
            return;
        }

        ShowQuestion(test.quesAnswers[currentQuestionIndex]);
    }

    void ShowQuestion(QuesAnswer qa)
    {
        questionPanel.SetActive(true);
        currentTime = questionTime;
        isCountingDown = true;
        UpdateTimerUI();

        questionText.text = qa.questions;

        for (int i = 0; i < optionToggles.Count; i++)
        {
            if (i < qa.options.Count)
            {
                optionLabels[i].text = qa.options[i];
                optionToggles[i].gameObject.SetActive(true);
                optionToggles[i].isOn = false;
                optionToggles[i].group = toggleGroup;
            }
            else
            {
                optionToggles[i].gameObject.SetActive(false);
            }
        }

        toggleGroup.SetAllTogglesOff(true);
    }

    // ==============================
    // ANSWER CHECK (FIXED)
    // ==============================

    public void CheckAnswer()
    {
        isCountingDown = false;

        var qa = quesData.tests[currentTestIndex].quesAnswers[currentQuestionIndex];

        if (string.IsNullOrEmpty(qa.answers))
        {
            Debug.LogError("❌ Answer missing for question: " + qa.questions);
            return;
        }

        string selectedOption = "";

        for (int i = 0; i < optionToggles.Count; i++)
        {
            if (optionToggles[i].isOn)
            {
                if (optionLabels[i] == null)
                {
                    Debug.LogError("❌ Option label missing at index " + i);
                    return;
                }

                selectedOption = optionLabels[i].text;
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(selectedOption))
        {
            answerText.text = "No option selected!";
            StartCoroutine(HideQuestionPanelAfterDelay());
            return;
        }

        bool isCorrect = string.Equals(
            selectedOption.Trim(),
            qa.answers.Trim(),
            System.StringComparison.OrdinalIgnoreCase
        );

        if (isCorrect)
        {
            if (soundManager != null)
                soundManager.PlaySFX("CorrectAnswer");

            answerText.text = "Correct Answer!";
            score++;
            UpdateScoreUI();

            if (gasBar != null)
                gasBar.AddGas(0.2f);

            if (car != null)
                car.ResumeDriving();

            currentQuestionIndex++;
        }
        else
        {
            if (soundManager != null)
                soundManager.PlaySFX("WrongAnswer");

            answerText.text = "Wrong Answer!";
            life--;
            UpdateWrongAnswersUI();

            if (car != null)
            {
                if (life == 2) car.MoveBackByWaypoints(3);
                else if (life == 1) car.MoveBackByWaypoints(6);
                else if (life <= 0)
                    StartCoroutine(RestartAfterDelay());
            }
        }

        StartCoroutine(HideQuestionPanelAfterDelay());
    }

    // ==============================
    // TEST FLOW
    // ==============================

    void StartNextTest()
    {
        currentTestIndex++;
        currentQuestionIndex = 0;

        if (currentTestIndex >= quesData.tests.Count)
        {
            if (Gameover != null) Gameover.SetActive(true);
            return;
        }

        if (soundManager != null)
            soundManager.PlayGameplayMusic();

        ShowNextQuestion();
    }

    IEnumerator HideQuestionPanelAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        questionPanel.SetActive(false);
    }

    // ==============================
    // UI
    // ==============================

    void UpdateScoreUI()
    {
        if (Scoretext != null)
            Scoretext.text = "Score: " + score;
    }

    void UpdateWrongAnswersUI()
    {
        if (wrongAnswersText != null)
            wrongAnswersText.text = "Life: " + Mathf.Max(0, life);

        if (life <= 0 && Gameover != null)
            Gameover.SetActive(true);
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    void RestartGame()
    {
        life = 3;
        score = 0;
        currentTestIndex = 0;
        currentQuestionIndex = 0;

        UpdateScoreUI();
        UpdateWrongAnswersUI();

        questionPanel.SetActive(false);
        Gameover.SetActive(false);

        if (GameManager.instance != null && GameManager.instance.car != null)
            GameManager.instance.car.RespawnAtStart();

        ShowNextQuestion();
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        RestartGame();
    }
}
