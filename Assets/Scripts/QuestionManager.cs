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

    [SerializeField] private int wrongAnswerCount = 0;

    private AICarController car;
    private GasBar gasBar;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        confirmButton.onClick.AddListener(CheckAnswer);

        car = FindObjectOfType<AICarController>();
        gasBar = FindObjectOfType<GasBar>();
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
            Debug.Log("Correct Answer!");
            questionPanel.SetActive(false);

            if (gasBar != null) 
                gasBar.AddGas(gasBar.gasFillAmount); // reward fuel

            // Resume car
            //AICarController car = FindObjectOfType<AICarController>();
            if (car != null) 
                car.ResumeDriving();

            // Disable collectible/checkpoint
            if (car != null) 
                car.DismissCollectible();
        }
        else
        {
            wrongAnswerCount++;

            if (car == null) return;

            if (wrongAnswerCount == 1)
            {
                car.MoveBackWaypoints(1);
            }
            else if (wrongAnswerCount == 2)
            {
                car.MoveBackWaypoints(2);
            }
            else if (wrongAnswerCount >= 3)
            {
                car.RespawnAtStart();
                wrongAnswerCount = 0;
            }

            Debug.Log("Wrong Answer!");
        }
    }
}
