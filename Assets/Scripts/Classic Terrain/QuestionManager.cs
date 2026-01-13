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
    //public QuestionData questionData;
    public QuesData quesData;

    [Header("UI References")]
    public GameObject questionPanel;
    //public Text questionText;
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

    //private int currentQuestionIndex = -1;
    private int currentTestIndex = 0;
    private int currentQuestionIndex = 0;
    //private int nextQuestionIndex = 0;
    private float currentTime;
    private bool isCountingDown = false;

    [Header("Game Data")]
    public int score = 0;
    public int life = 3;

    private Car car;
    private GasBar gasBar;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        car = FindObjectOfType<Car>();
        gasBar = FindObjectOfType<GasBar>();
        UpdateScoreUI();
        UpdateWrongAnswersUI();
    }

    void Update()
    {
        if (isCountingDown)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                isCountingDown = false;
                CheckAnswer();
            }
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
        }
    }

    //public void ShowNextQuestion()
    //{
    //    ShowQuestion(nextQuestionIndex);
    //}
    public void ShowNextQuestion()
    {
        if (currentTestIndex >= quesData.tests.Count)
        {
            Debug.Log("All tests completed!");
            if (Gameover != null) Gameover.SetActive(true);
            return;
        }

        var currentTest = quesData.tests[currentTestIndex];

        if (currentQuestionIndex >= currentTest.quesAnswers.Count)
        {
            StartNextTest();
            return;
        }

        ShowQuestion(currentTest.quesAnswers[currentQuestionIndex]);
    }



    //public void ShowQuestion(int index)
    //{
    //    currentQuestionIndex = index;

    //    if (index >= questionData.questionAnswers.Count)
    //    {
    //        Debug.Log("No more questions!");
    //        return;
    //    }

    //    questionPanel.SetActive(true);
    //    currentTime = questionTime;
    //    isCountingDown = true;
    //    UpdateTimerUI();

    //    var qa = questionData.questionAnswers[index];
    //    questionText.text = qa.questions;

    //    for (int i = 0; i < optionLabels.Count; i++)
    //    {
    //        if (i < qa.options.Count)
    //        {
    //            optionLabels[i].text = qa.options[i];
    //            optionToggles[i].gameObject.SetActive(true);
    //            optionToggles[i].isOn = false;
    //            if (toggleGroup) optionToggles[i].group = toggleGroup;
    //        }
    //        else
    //        {
    //            optionToggles[i].gameObject.SetActive(false);
    //        }
    //    }

    //    if (toggleGroup) toggleGroup.SetAllTogglesOff(true);
    //}
    public void ShowQuestion(QuesAnswer qa)
    {
        questionPanel.SetActive(true);
        currentTime = questionTime;
        isCountingDown = true;
        UpdateTimerUI();

        questionText.text = qa.questions;

        for (int i = 0; i < optionLabels.Count; i++)
        {
            if (i < qa.options.Count)
            {
                optionLabels[i].text = qa.options[i];
                optionToggles[i].gameObject.SetActive(true);
                optionToggles[i].isOn = false;
                if (toggleGroup) optionToggles[i].group = toggleGroup;
            }
            else
            {
                optionToggles[i].gameObject.SetActive(false);
            }
        }

        if (toggleGroup) toggleGroup.SetAllTogglesOff(true);
    }


    //public void CheckAnswer()
    //{
    //    isCountingDown = false;

    //    var qa = questionData.questionAnswers[currentQuestionIndex];
    //    string selectedOption = "";

    //    for (int i = 0; i < optionToggles.Count; i++)
    //    {
    //        if (optionToggles[i].isOn)
    //        {
    //            selectedOption = optionLabels[i].text;
    //            break;
    //        }
    //    }

    //    if (string.IsNullOrWhiteSpace(selectedOption))
    //    {
    //        answerText.text = "No option selected!";
    //        return;
    //    }

    //    bool isCorrect = string.Equals(selectedOption.Trim(), qa.answers.Trim(), System.StringComparison.OrdinalIgnoreCase);

    //    if (isCorrect)
    //    {
    //        answerText.text = "Correct Answer!";
    //        nextQuestionIndex++;
    //        score++;
    //        UpdateScoreUI();

    //        if (gasBar != null) gasBar.AddGas(0.2f);

    //        if (car != null) car.ResumeDriving();
    //    }
    //    else
    //    {
    //        answerText.text = "Wrong Answer!";
    //        life--;
    //        UpdateWrongAnswersUI();

    //        // ❌ do NOT respawn at start
    //        // GameManager.instance.car.RespawnAtStart();

    //        // ✅ punishment: move back 10 waypoints
    //        if (car != null)
    //        {


    //            car.MoveBackByWaypoints(3);
    //            life--;
    //            car.ResumeDriving();
    //        }
    //    }


    //    StartCoroutine(HideQuestionPanelAfterDelay());
    //}
    public void CheckAnswer()
    {
        isCountingDown = false;

        var qa = quesData.tests[currentTestIndex].quesAnswers[currentQuestionIndex];
        string selectedOption = "";

        for (int i = 0; i < optionToggles.Count; i++)
        {
            if (optionToggles[i].isOn)
            {
                selectedOption = optionLabels[i].text;
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(selectedOption))
        {
            answerText.text = "No option selected!";
            return;
        }

        bool isCorrect = string.Equals(
            selectedOption.Trim(),
            qa.answers.Trim(),
            System.StringComparison.OrdinalIgnoreCase
        );

        if (isCorrect)
        {
            answerText.text = "Correct Answer!";
            score++;
            UpdateScoreUI();
            if (gasBar != null) gasBar.AddGas(0.2f);
            if (car != null) car.ResumeDriving();
        }
        else
        {
            answerText.text = "Wrong Answer!";
            life--;
            UpdateWrongAnswersUI();
            GameManager.instance.car.RespawnAtStart();
        }

        currentQuestionIndex++;
        StartCoroutine(HideQuestionPanelAfterDelay());
    }

    private void StartNextTest()
    {
        currentTestIndex++;
        currentQuestionIndex = 0;

        if (currentTestIndex >= quesData.tests.Count)
        {
            Debug.Log("All tests finished!");
            if (Gameover != null) Gameover.SetActive(true);
            return;
        }

        Debug.Log("Starting Test: " + quesData.tests[currentTestIndex].testsName);
        ShowNextQuestion();
    }


    private IEnumerator HideQuestionPanelAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        questionPanel.SetActive(false);
    }

    public void UpdateScoreUI()
    {
        if (Scoretext != null)
            Scoretext.text = "Score: " + score;
    }

    public void UpdateWrongAnswersUI()
    {
        if (wrongAnswersText != null)
            wrongAnswersText.text = "Life: " + Mathf.Max(0, life);

        if (life <= 0)
        {
            if (Gameover != null) Gameover.SetActive(true);
        }
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }


    //This is for asads
}
