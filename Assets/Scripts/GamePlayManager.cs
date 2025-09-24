
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Car Reference")]
    public AICarController car;   // Drag your car here

    [Header("Wrong Answer Tracking")]
    private int wrongAnswers = 0;

    private void Awake()
    {
        // Singleton (so we can access GameManager.instance)
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Called when the player gives a wrong answer
    /// </summary>
    public void OnWrongAnswer()
    {
        wrongAnswers++;

        if (wrongAnswers == 1)
        {
            MoveCarBack(1);
        }
        else if (wrongAnswers == 2)
        {
            MoveCarBack(2);
        }
        else if (wrongAnswers >= 3)
        {
            RespawnAtStart();
            wrongAnswers = 0; // reset counter
        }
    }

    private void MoveCarBack(int steps)
    {
        if (car != null)
        {
            car.MoveBackWaypoints(steps);
        }
    }

    private void RespawnAtStart()
    {
        if (car != null)
        {
            car.RespawnAtStart();
        }
    }
}

