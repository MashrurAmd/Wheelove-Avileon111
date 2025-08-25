using UnityEngine;
using UnityEngine.UI;

public class GasBar : MonoBehaviour
{
    public Image gasBarImage;          // Drag your UI Image (Gas Bar) here
    public Text gasText;               // Drag your UI Text here (for % display)

    public float maxGas = 1f;          // 1 = full tank
    public float currentGas = 1f;      // starts full
    public float gasConsumptionRate = 0.01f; // decrease speed per sec
    public float gasFillAmount;

    [Header("Respawn Settings")]
    public Transform startPoint;   // Drag an Empty GameObject at start
    private AICarController car;   // Reference to car

    void Start()
    {
        car = FindObjectOfType<AICarController>();
        gasBarImage.fillAmount = currentGas;
        UpdateGasText();
    }

    void Update()
    {
        if (AICarController.isCarMoving)
        {
            currentGas -= gasConsumptionRate * Time.deltaTime;
            currentGas = Mathf.Clamp01(currentGas);
            gasBarImage.fillAmount = currentGas;

            UpdateGasText();

            // 🚨 Out of gas → respawn
            if (currentGas <= 0f)
            {
                RespawnCar();
            }
        }
    }

    public void AddGas(float amount)
    {
        currentGas = Mathf.Clamp01(currentGas + amount);
        gasBarImage.fillAmount = currentGas;
        UpdateGasText();
    }

    private void RespawnCar()
    {
        if (startPoint != null && car != null)
        {
            car.transform.position = startPoint.position;
            car.transform.rotation = startPoint.rotation;

            Rigidbody rb = car.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            currentGas = maxGas;
            gasBarImage.fillAmount = currentGas;
            UpdateGasText();
        }
    }

    private void UpdateGasText()
    {
        if (gasText != null)
        {
            int percentage = Mathf.RoundToInt(currentGas * 100f);
            gasText.text = percentage + "%";
        }
    }
}


