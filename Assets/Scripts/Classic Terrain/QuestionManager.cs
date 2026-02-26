using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

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
    public List<Image> emojisImages;
    public AndroidTTS tts;


}

public class QuestionManager : MonoBehaviour
{
    [Header("Data")]
    public QuesData englishQuesData;    // ← assign English scriptable object
    public QuesData hebrewQuesData;


    [Header("UI Sets")]
    public QuestionUISet portraitUI;
    public QuestionUISet landscapeUI;

    [Header("Level Settings")]
    public int levelTestIndex = 0;

    private QuestionUISet ui;

    [Header("Timer")]
    public float questionTime = 15f;

    [Header("Level Timer Settings")]
    public bool useTimer = false;
    public float timedLevelDuration = 15f;

    private int currentQuestionIndex = 0;
    private float currentTime;
    private bool isCountingDown = false;

    private bool isSceneUnloading = false;

    private SoundManager soundManager;
    private Car car;
    private GasBar gasBar;

    [Header("Game Data")]
    public int score = 0;
    public int life = 3;

    bool lastLandscape;


    private QuesData quesData;




    [Header("Wrong Answer Tracking")]
    private int wrongAnswerCount = 0;

    // ============================
    // UNITY EVENTS
    // ============================

    void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void OnSceneUnloaded(Scene scene)
    {
        isSceneUnloading = true;
    }

    void Start()
    {
        // Pick language based on selection
        if (LocalizationManager.isHebrew && hebrewQuesData != null)
            quesData = hebrewQuesData;
        else
            quesData = englishQuesData;

        car = FindObjectOfType<Car>();
        gasBar = FindObjectOfType<GasBar>();
        soundManager = FindObjectOfType<SoundManager>();

        UpdateScoreUI();
        UpdateWrongAnswersUI();
    }



    void Update()
    {
        UpdateUISet();

        if (!isCountingDown || isSceneUnloading) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isCountingDown = false;
            HandleTimeUp();
        }


        UpdateTimerUI();
    }

    void HandleTimeUp()
    {
        if (!useTimer)
            return;

        if (ui != null)
            ui.answerText.text = "Time's Up!";

        // Move back 3 waypoints
        if (car != null)
            car.MoveBackByWaypoints(3);

        // Deduct score
        score = Mathf.Max(0, score - 1);
        UpdateScoreUI();

        StartCoroutine(HideQuestionPanelAfterDelay());
    }


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
        if (ui != null && ui.timerText != null)
            ui.timerText.text = Mathf.CeilToInt(currentTime).ToString();
    }

    // ============================
    // QUESTION FLOW
    // ============================

    public void ShowNextQuestion()
    {
        if (isSceneUnloading) return;
        if (ui == null || ui.questionPanel == null) return;
        if (levelTestIndex >= quesData.tests.Count) return;

        var test = quesData.tests[levelTestIndex];

        if (currentQuestionIndex >= test.quesAnswers.Count)
        {
            StartNextTest();
            return;
        }

        ShowQuestion(test.quesAnswers[currentQuestionIndex]);
    }

    void ShowQuestion(QuesAnswer qa)
    {
        if (isSceneUnloading) return;
        if (ui == null || ui.questionPanel == null) return;

        if (car != null)
            car.PauseCar();

        GameObject panel = ui.questionPanel;
        panel.SetActive(true);

        RectTransform rt = panel.GetComponent<RectTransform>();
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();

        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        // Kill previous tweens to prevent overlap
        rt.DOKill();
        cg.DOKill();

        // Reset state
        rt.localScale = Vector3.zero;
        cg.alpha = 0f;

        // 🔥 Smooth professional open animation
        rt.DOScale(Vector3.one, 0.5f)
          .SetEase(Ease.OutBack);

        cg.DOFade(1f, 0.4f);



        if (useTimer)
        {
            currentTime = timedLevelDuration;
            isCountingDown = true;
            UpdateTimerUI();
        }
        else
        {
            isCountingDown = false;

            if (ui.timerText != null)
                ui.timerText.text = "";
        }


        ui.questionText.text = qa.questions;

        //ui.tts.Speak(qa.questions);     // tts added here
        //AndroidTTS.instance.Speak(qa.questions);
        StartCoroutine(SpeakQuestionAndOptions(qa));

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

    // tts method to speak question and options sequentially with dynamic timing
    IEnumerator SpeakQuestionAndOptions(QuesAnswer qa)
    {
        if (AndroidTTS.instance == null)
            yield break;

        // 🔹 Speak Question First
        AndroidTTS.instance.Speak(qa.questions);

        // Wait depending on question length (dynamic delay)
        float questionDelay = Mathf.Clamp(qa.questions.Length * 0.05f, 2f, 6f);
        yield return new WaitForSeconds(questionDelay);

        // 🔹 Speak Options One By One
        for (int i = 0; i < qa.options.Count; i++)
        {
            string optionText = qa.options[i];

            // Optional: Add numbering voice
            string speakText = "Option " + (i + 1) + ". " + optionText;

            AndroidTTS.instance.Speak(speakText);

            float optionDelay = Mathf.Clamp(optionText.Length * 0.05f, 1.5f, 4f);
            yield return new WaitForSeconds(optionDelay);
        }
    }

    // read current question using TTS (can be called from a button)
    public void ReadCurrentQuestion()
    {
        if (levelTestIndex >= quesData.tests.Count) return;
        if (currentQuestionIndex >= quesData.tests[levelTestIndex].quesAnswers.Count) return;

        var qa = quesData.tests[levelTestIndex].quesAnswers[currentQuestionIndex];
        AndroidTTS.instance.Speak(qa.questions);
    }



    // ============================
    // ANSWER CHECK
    // ============================

    public void CheckAnswer()
    {
        if (isSceneUnloading) return;

        isCountingDown = false;

        var qa = quesData.tests[levelTestIndex].quesAnswers[currentQuestionIndex];

        string selectedOption = "";

        for (int i = 0; i < ui.optionToggles.Count; i++)
        {
            if (ui.optionToggles[i].isOn)
            {
                selectedOption = ui.optionLabels[i].text;
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(selectedOption))
        {
            ui.answerText.text = "No option selected!";

            if (car != null)
            {
                car.MoveBackByWaypoints(3); // same penalty
                car.ResumeDriving();        // ⭐ IMPORTANT
            }

            StartCoroutine(HideQuestionPanelAfterDelay());
            return;
        }


        bool isCorrect = selectedOption.Trim().ToLower() ==
                         qa.answers.Trim().ToLower();

        if (isCorrect)
        {
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
            if (!isCorrect)
            {
                ui.answerText.text = "Wrong Answer!";
                life--;
                UpdateWrongAnswersUI();

                if (car != null)
                {
                    wrongAnswerCount++; // increment wrong answer counter

                    if (wrongAnswerCount == 1)
                    {
                        car.MoveBackByWaypoints(3); // 1st wrong answer
                    }
                    else if (wrongAnswerCount == 2)
                    {
                        car.MoveBackByWaypoints(7); // 2nd wrong answer
                    }
                    else
                    {
                        car.RespawnAtStart(); // 3rd wrong answer → reset to start
                    }

                    car.ResumeDriving();
                }

                if (life <= 0)
                    StartCoroutine(RestartAfterDelay());
            }
        }

        StartCoroutine(HideQuestionPanelAfterDelay());
    }

    void StartNextTest()
    {
        if (ui != null && ui.gameOver != null)
            ui.gameOver.SetActive(true);
    }

    IEnumerator HideQuestionPanelAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        if (ui == null || ui.questionPanel == null)
            yield break;

        GameObject panel = ui.questionPanel;

        RectTransform rt = panel.GetComponent<RectTransform>();
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();

        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        rt.DOKill();
        cg.DOKill();

        // 🔥 Smooth close animation
        rt.DOScale(Vector3.zero, 0.3f)
          .SetEase(Ease.InBack);

        cg.DOFade(0f, 0.25f);

        yield return new WaitForSeconds(0.3f);

        panel.SetActive(false);
    }



    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ============================
    // UI
    // ============================

    void UpdateScoreUI()
    {
        if (ui != null && ui.scoreText != null)
            ui.scoreText.text = "Score: " + score;
    }

    void UpdateWrongAnswersUI()
    {
        if (ui != null && ui.wrongAnswersText != null)
            ui.wrongAnswersText.text = "Life: " + Mathf.Max(0, life);
    }

    public void OnSceneReloaded()
    {
        // Reassign scene-specific references
        car = FindObjectOfType<Car>();
        gasBar = FindObjectOfType<GasBar>();
        soundManager = FindObjectOfType<SoundManager>();

        // Reset question/level variables
        currentQuestionIndex = 0;
        wrongAnswerCount = 0;
        life = 3;
        score = 0;

        UpdateScoreUI();
        UpdateWrongAnswersUI();

        // DO NOT call ShowNextQuestion here
    }

}
