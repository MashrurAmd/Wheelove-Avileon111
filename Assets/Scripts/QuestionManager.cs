using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;

    [Header("Data")]
    public QuestionData questionData;

    [Header("UI References")]
    public GameObject questionPanel;
    public Text questionText;
    public List<Toggle> optionToggles;
    public List<Text> optionLabels;
    public Button confirmButton;
    public ToggleGroup toggleGroup;

    private int currentQuestionIndex = -1;
    private int nextQuestionIndex = 0;
    private int activeCheckpointIndex = -1;

    [Header("Timer")]
    public TMP_Text timerText;
    public float questionTime = 10f;
    private float currentTime;
    private bool isCountingDown = false;

    public int wrongAnswerCount = 0;
    public TMP_Text answerText;

    private AICarController car;
    private GasBar gasBar;

    public int score = 0;
    public Text Scoretext;

    public int life = 3;
    public Text wrongAnswersText;

    public GameObject Gameover;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        car = FindObjectOfType<AICarController>();
        gasBar = FindObjectOfType<GasBar>();
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
                CheckAnswer(); // Auto check when timer ends
            }
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text =  Mathf.CeilToInt(currentTime).ToString();
        }
    }

    public void ShowNextQuestion()
    {
        ShowQuestion(nextQuestionIndex);
    }

    public void ShowQuestion(int index)
    {
        activeCheckpointIndex = index;
        currentQuestionIndex = index;

        if (index >= questionData.questionAnswers.Count)
        {
            Debug.Log("No more questions!");
            return;
        }

        questionPanel.SetActive(true);

        // 🔑 Start the countdown whenever the panel shows
        currentTime = questionTime;
        isCountingDown = true;
        UpdateTimerUI();

        // Load Question + Options
        var qa = questionData.questionAnswers[index];
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

    public void CheckAnswer()
    {
        isCountingDown = false; // 🛑 stop timer once answer is checked

        var qa = questionData.questionAnswers[currentQuestionIndex];

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
            Debug.Log("No option selected!");
            answerText.text = "No option selected!";
            return;
        }

        bool isCorrect = string.Equals(selectedOption.Trim(), qa.answers.Trim(),
            StringComparison.OrdinalIgnoreCase);

        if (isCorrect)
        {
            Debug.Log("Correct Answer!");
            answerText.text = "Correct Answer!";

            if (gasBar != null) gasBar.AddGas(gasBar.gasFillAmount);
            nextQuestionIndex++;

            if (car != null)
            {
                car.DismissCollectible();
                car.ResumeDriving();
            }

            score++;
            UpdateScoreUI();
        }
        else
        {
            wrongAnswerCount++;

            if (car != null)
            {
                if (wrongAnswerCount == 1) car.MoveBackWaypoints(1);
                else if (wrongAnswerCount == 2) car.MoveBackWaypoints(2);
                else if (wrongAnswerCount >= 3) { car.RespawnAtStart(); wrongAnswerCount = 0; }
            }

            answerText.text = "Wrong Answer!";
            Debug.Log("Wrong Answer!");

            life--;
            UpdateWrongAnswersUI();
        }

        StartCoroutine(WaitQuestionPanelDisableSeq());
    }

    private IEnumerator WaitQuestionPanelDisableSeq()
    {
        yield return new WaitForSeconds(1);
        questionPanel.SetActive(false);
    }

    public void UpdateScoreUI()
    {
        if (Scoretext != null)
        {
            Scoretext.text = "Score : " + score.ToString();
        }
    }

    public void UpdateWrongAnswersUI()
    {
        if (wrongAnswersText != null)
        {
            wrongAnswersText.text = "Life : " + life.ToString();
        }

        if (life <= 0)
        {
            Debug.Log("Game Over!");
            Gameover.SetActive(true);
        }
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
