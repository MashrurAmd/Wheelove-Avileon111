using UnityEngine;

public class AnswerManager : MonoBehaviour
{
    private AICarController car;
    private GasBar gasBar;

    public int wrongAnswerCount = 0; // Track wrong attempts for current question

    private void Start()
    {
        car = FindObjectOfType<AICarController>();
        gasBar = FindObjectOfType<GasBar>();
    }

    // Call this from your "Correct" button
    public void OnCorrectAnswer()
    {
        if (gasBar != null)
            gasBar.AddGas(0.1f); // reward fuel

        if (car != null)
        {
            car.DismissCollectible();   
            car.ResumeDriving();        
        }

        
    }

    // Call this from your "Wrong" button
    public void OnWrongAnswer()
    {
        wrongAnswerCount++;

        if (car == null) return;

        if (wrongAnswerCount == 1)
        {
            car.MoveBackWaypoints(1); // Go back 1 checkpoint
        }
        else if (wrongAnswerCount == 2)
        {
            car.MoveBackWaypoints(2); // Go back 2 checkpoints
        }
        else if (wrongAnswerCount >= 3)
        {
            car.RespawnAtStart(); // Full restart
            wrongAnswerCount = 0; // reset again
        }
    }
}
