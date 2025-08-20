using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // if you are using TextMeshPro

public class QuestionManager : MonoBehaviour
{
    [Header("Data")]
    public QuestionData questionData; // ScriptableObject with questions

    [Header("UI References")]
    public GameObject questionPanel;
    public Text questionText; // use Text if not TMP
    public List<Toggle> optionToggles; // 3 toggles
    public List<Text> optionLabels; // option texts
    public Button confirmButton;
    public ToggleGroup toggleGroup;

    [Header("Gameplay References")]
    private AICarController car;
    private GasBar gasBar;

    private int currentQuestionIndex = -1;
    private int wrongAnswerCount = 0;

    private void Start()
    {
        car = FindObjectOfType<AICarController>();
        gasBar = FindObjectOfType<GasBar>();

        questionPanel.SetActive(false);
        confirmButton.onClick.AddListener(OnConfirmAnswer);
    }

    // Called when car hits checkpoint
    public void ShowNextQuestion()
    {
        currentQuestionIndex++;

        if (currentQuestionIndex >= questionData.questionAnswers.Count)
        {
            Debug.Log("All questions completed!");
            return;
        }

        questionPanel.SetActive(true);
        DisplayQuestion();
    }

    private void DisplayQuestion()
    {
        var q = questionData.questionAnswers[currentQuestionIndex];

        questionText.text = q.questions;

        for (int i = 0; i < optionToggles.Count; i++)
        {
            if (i < q.options.Count)
            {
                optionToggles[i].gameObject.SetActive(true);
                optionLabels[i].text = q.options[i];
                optionToggles[i].isOn = false;
            }
            else
            {
                optionToggles[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnConfirmAnswer()
    {
        Toggle selected = null;

        foreach (var toggle in optionToggles)
        {
            if (toggle.isOn)
            {
                selected = toggle;
                break;
            }
        }

        if (selected == null)
        {
            Debug.Log("No option selected!");
            return;
        }

        string selectedAnswer = optionLabels[optionToggles.IndexOf(selected)].text;
        string correctAnswer = questionData.questionAnswers[currentQuestionIndex].answers;

        if (selectedAnswer == correctAnswer)
            OnCorrectAnswer();
        else
            OnWrongAnswer();
    }

    private void OnCorrectAnswer()
    {
        if (gasBar != null)
            gasBar.AddGas(0.1f);

        if (car != null)
            car.ResumeDriving();

        questionPanel.SetActive(false);
    }

    private void OnWrongAnswer()
    {
        wrongAnswerCount++;

        if (car == null) return;

        if (wrongAnswerCount == 1)
            car.MoveBackWaypoints(1);
        else if (wrongAnswerCount == 2)
            car.MoveBackWaypoints(2);
        else if (wrongAnswerCount >= 3)
        {
            car.RespawnAtStart();
            wrongAnswerCount = 0;
        }

        questionPanel.SetActive(false);
    }
}
