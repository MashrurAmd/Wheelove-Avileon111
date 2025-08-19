using UnityEngine;
using UnityEngine.UI;

public class GasBar : MonoBehaviour
{
    public Image gasFill;         // Assign your GasBar image here in Inspector
    public float maxGas = 1f;     // Full tank = 1 (100%)
    public float currentGas = 1f; // Start with full gas
    public float gasConsumptionRate = 0.05f; // Per second when moving



    void Update()
    {
        // Reduce gas if car is moving
        if (AICarController.isCarMoving)
        {
            currentGas -= gasConsumptionRate * Time.deltaTime;
            currentGas = Mathf.Clamp01(currentGas);
            gasFill.fillAmount = currentGas;
        }
    }

    // Called from AICarController when correct answer is confirmed
    public void AddGas(float amount)
    {
        currentGas = Mathf.Clamp01(currentGas + amount);
        gasFill.fillAmount = currentGas;
    }
}
