using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class QuestionUISet
{
    public GameObject questionPanel;
    public TMP_Text questionText;
    public List<Toggle> optionToggles;
    public List<Text> optionLabels;
    public ToggleGroup toggleGroup;
    public TMP_Text timerText;
    public TMP_Text answerText;
    public Text scoreText;
    public Text wrongAnswersText;
    public GameObject gameOver;
    //public Image emojisImage;
    public List<Image> emojisImages;

}

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;

    [Header("Data")]
    public QuesData quesData;

    [Header("UI Sets")]
    public QuestionUISet portraitUI;
    public QuestionUISet landscapeUI;

    QuestionUISet ui;

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
        UpdateUISet();      //added new

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
            //soundManager.PlayGameplayMusic();
            SoundManager.Instance.PlayMusic("ClassicModeMusic");

        // ❌ DO NOT show question here
    }


    void Update()
    {
        UpdateUISet();      //added new

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

    //void UpdateUISet()      //added new
    //{
    //    bool isLandscape = Screen.width > Screen.height;
    //    ui = isLandscape ? landscapeUI : portraitUI;
    //}
    bool lastLandscape;
    void UpdateUISet()
    {
        bool isLandscape = Screen.width > Screen.height;

        if (ui == null || isLandscape != lastLandscape)
        {
            ui = isLandscape ? landscapeUI : portraitUI;
            lastLandscape = isLandscape;
        }
    }

    void UpdateTimerUI()
    {
        if (ui.timerText != null)
            ui.timerText.text = Mathf.CeilToInt(currentTime).ToString();
    }

    // ==============================
    // QUESTION FLOW
    // ==============================

    public void ShowNextQuestion()
    {
        if (currentTestIndex >= quesData.tests.Count)
        {
            if (ui.gameOver != null) ui.gameOver.SetActive(true);
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
        ui.questionPanel.SetActive(true);
        currentTime = questionTime;
        isCountingDown = true;
        UpdateTimerUI();

        ui.questionText.text = qa.questions;

        for (int i = 0; i < ui.optionToggles.Count; i++)
        {
            if (i < qa.options.Count)
            {
                ui.optionLabels[i].text = qa.options[i];
                ui.optionToggles[i].gameObject.SetActive(true);
                ui.optionToggles[i].isOn = false;
                ui.optionToggles[i].group = ui.toggleGroup;
            }
            else
            {
                ui.optionToggles[i].gameObject.SetActive(false);
            }
        }

        ui.toggleGroup.SetAllTogglesOff(true);

        // Set emojis
        for (int i = 0; i < ui.emojisImages.Count; i++)
        {
            if (qa.emogis != null && i < qa.emogis.Length)
            {
                ui.emojisImages[i].sprite = qa.emogis[i];
                ui.emojisImages[i].gameObject.SetActive(true);
            }
            else
            {
                ui.emojisImages[i].gameObject.SetActive(false);
            }
        }
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

        for (int i = 0; i < ui.optionToggles.Count; i++)
        {
            if (ui.optionToggles[i].isOn)
            {
                if (ui.optionLabels[i] == null)
                {
                    Debug.LogError("❌ Option label missing at index " + i);
                    return;
                }

                selectedOption = ui.optionLabels[i].text;
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(selectedOption))
        {
            ui.answerText.text = "No option selected!";
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

            ui.answerText.text = "Correct Answer!";
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

            ui.answerText.text = "Wrong Answer!";
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
            if (ui.gameOver != null) ui.gameOver.SetActive(true);
            return;
        }

        if (soundManager != null)
            //soundManager.PlayGameplayMusic();
            SoundManager.Instance.PlayMusic("ClassicModeMusic");

        ShowNextQuestion();
    }

    IEnumerator HideQuestionPanelAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        ui.questionPanel.SetActive(false);
    }

    // ==============================
    // UI
    // ==============================

    void UpdateScoreUI()
    {
        if (ui.scoreText != null)
            ui.scoreText.text = "Score: " + score;
    }

    void UpdateWrongAnswersUI()
    {
        if (ui.wrongAnswersText != null)
            ui.wrongAnswersText.text = "Life: " + Mathf.Max(0, life);

        if (life <= 0 && ui.gameOver != null)
            ui.gameOver.SetActive(true);
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

        ui.questionPanel.SetActive(false);
        ui.gameOver.SetActive(false);

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
