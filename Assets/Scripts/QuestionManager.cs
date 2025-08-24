using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro; // optional

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

    public int wrongAnswerCount = 0;
    public TMP_Text answerText;

    private AICarController car;
    private GasBar gasBar;

    void Awake() { if (Instance == null) Instance = this; }

    void Start()
    {
        //confirmButton.onClick.AddListener(CheckAnswer);
        car = FindObjectOfType<AICarController>();
        gasBar = FindObjectOfType<GasBar>();
    }

    // Called by the car when it hits a checkpoint
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
            //questionPanel.SetActive(false);
            return;
        }

        questionPanel.SetActive(true);

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
        var qa = questionData.questionAnswers[currentQuestionIndex];

        // Find selected option
        string selectedOption = "";
        for (int i = 0; i < optionToggles.Count; i++)
            if (optionToggles[i].isOn) { selectedOption = optionLabels[i].text; break; }

        if (string.IsNullOrWhiteSpace(selectedOption))
        {
            Debug.Log("No option selected!");
            answerText.text = "No option selected!";
            return;
        }

        bool isCorrect = string.Equals(
            selectedOption.Trim(), qa.answers.Trim(),
            StringComparison.OrdinalIgnoreCase);

        if (isCorrect)
        {
            Debug.Log("Correct Answer!");
            answerText.text = "Correct Answer!";

            if (gasBar != null) gasBar.AddGas(gasBar.gasFillAmount);

            // move to the next question ONLY on correct
            nextQuestionIndex++;

            if (car != null)
            {
                car.DismissCollectible();
                car.ResumeDriving();
            }
        }
        else
        {
            wrongAnswerCount++;

            if (car == null) return;

            // Move back, then ResumeDriving() is called by those methods
            if (wrongAnswerCount == 1) car.MoveBackWaypoints(1);
            else if (wrongAnswerCount == 2) car.MoveBackWaypoints(2);
            else if (wrongAnswerCount >= 3) { car.RespawnAtStart(); wrongAnswerCount = 0; }
            answerText.text = "Wrong Answer!";
            Debug.Log("Wrong Answer!");
        }

        StartCoroutine(WaitQuestionPanelDisableSeq());
    }

    private IEnumerator WaitQuestionPanelDisableSeq()
    {
        yield return new WaitForSeconds(1);
        questionPanel.SetActive(false);
    }
}