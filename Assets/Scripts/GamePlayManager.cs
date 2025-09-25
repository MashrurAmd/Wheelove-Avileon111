using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Car Reference")]
    public AICarController car;  // assign your car

    private int wrongAnswers = 0;

    // Call this when player gives a wrong answer
    //public void WrongAnswer()
    //{
    //    wrongAnswers++;

    //    int totalWaypoints = WaypointManager.waypoints.Count;

    //    if (wrongAnswers >= 3)
    //    {
    //        // 3 wrong answers → go back to start
    //        car.TeleportTo(WaypointManager.waypoints[0].position, WaypointManager.waypoints[0].rotation);
    //        wrongAnswers = 0;
    //        Debug.Log("3 wrong answers! Car reset to start.");
    //        return;
    //    }

    //    // Move back number of waypoints equal to wrongAnswers
    //    int currentIndex = FindClosestWaypointIndex(car.GetPathPosition());
    //    int newIndex = Mathf.Max(currentIndex - wrongAnswers, 0);

    //    car.TeleportTo(WaypointManager.waypoints[newIndex].position,
    //                   WaypointManager.waypoints[newIndex].rotation);

    //    Debug.Log($"Wrong answer! Car moved back to waypoint {newIndex}");
    //}

    private int FindClosestWaypointIndex(float carPathPos)
    {
        int closestIndex = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < WaypointManager.waypoints.Count; i++)
        {
            float distance = Vector3.Distance(WaypointManager.waypoints[i].position,
                                              car.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
}
