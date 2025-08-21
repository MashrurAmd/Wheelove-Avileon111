using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private int currentQuestionIndex = -1;
    private int activeCheckpointIndex = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        confirmButton.onClick.AddListener(CheckAnswer);
    }

    public void ShowQuestion(int index)
    {
        activeCheckpointIndex = index;
        currentQuestionIndex = index;

        if (index >= questionData.questionAnswers.Count)
        {
            Debug.Log("No more questions!");
            questionPanel.SetActive(false);
            return;
        }

        questionPanel.SetActive(true);

        // Load Question
        QuestionAnswer qa = questionData.questionAnswers[index];
        questionText.text = qa.questions;

        // Load Options
        for (int i = 0; i < optionLabels.Count; i++)
        {
            if (i < qa.options.Count)
            {
                optionLabels[i].text = qa.options[i];
                optionToggles[i].gameObject.SetActive(true);
                optionToggles[i].isOn = false;
            }
            else
            {
                optionToggles[i].gameObject.SetActive(false);
            }
        }
    }

    void CheckAnswer()
    {
        QuestionAnswer qa = questionData.questionAnswers[currentQuestionIndex];
        string selectedOption = "";

        for (int i = 0; i < optionToggles.Count; i++)
        {
            if (optionToggles[i].isOn)
            {
                selectedOption = optionLabels[i].text;
                break;
            }
        }
        if (selectedOption == qa.answers)
        {
            Debug.Log("✅ Correct Answer!");
            questionPanel.SetActive(false);

            // Resume car
            AICarController car = FindObjectOfType<AICarController>();
            if (car != null) car.ResumeDriving();

            // Disable collectible/checkpoint
            if (car != null) car.DismissCollectible();
        }

        else
        {
            Debug.Log("❌ Wrong Answer! Try again.");
        }
    }
}
