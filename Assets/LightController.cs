using UnityEngine;
using System.Collections;

namespace HealthbarGames
{
    public class TrafficLightController : MonoBehaviour
    {
        public TrafficLightBase trafficLight;

        [Header("Timing (seconds)")]
        public float redDuration = 5f;
        public float yellowDuration = 2f;
        public float greenDuration = 5f;

        [Header("Cube")]
        public CubeMover cubeMover;

        void Start()
        {
            StartCoroutine(RunTrafficLight());
        }

        IEnumerator RunTrafficLight()
        {
            while (true)
            {
                // RED ← cube moves
                trafficLight.OnLightStateChanged(true, false, false);
                if (cubeMover != null) cubeMover.SetRedLight(true);
                yield return new WaitForSeconds(redDuration);

                // YELLOW ← cube stops
                trafficLight.OnLightStateChanged(false, true, false);
                if (cubeMover != null) cubeMover.SetRedLight(false);
                yield return new WaitForSeconds(yellowDuration);

                // GREEN ← cube stops
                trafficLight.OnLightStateChanged(false, false, true);
                if (cubeMover != null) cubeMover.SetRedLight(false);
                yield return new WaitForSeconds(greenDuration);

                // YELLOW ← cube stops
                trafficLight.OnLightStateChanged(false, true, false);
                if (cubeMover != null) cubeMover.SetRedLight(false);
                yield return new WaitForSeconds(yellowDuration);
            }
        }
    }
}