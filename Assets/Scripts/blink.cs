using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GasButtonBlink : MonoBehaviour
{
    [Header("References")]
    public Image targetImage;
    public Sprite greenSprite;
    public Sprite redSprite;
    public TriggerZone carTriggerZone; // SAME zone used in SimulationManager

    [Header("Blink Settings")]
    public float blinkSpeed = 0.5f;   // Lower = faster blink
    public float dimAlpha = 0.3f;
    public float brightAlpha = 1f;

    private Coroutine blinkRoutine;
    private bool isBlinking = false;

    void Update()
    {
        if (carTriggerZone.isTriggered)
        {
            if (!isBlinking)
            {
                StartRedBlink();
            }
        }
        else
        {
            StopRedBlink();
        }
    }

    void StartRedBlink()
    {
        isBlinking = true;
        targetImage.sprite = redSprite;

        blinkRoutine = StartCoroutine(BlinkEffect());
    }

    void StopRedBlink()
    {
        if (!isBlinking) return;

        isBlinking = false;

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        // Reset to green
        targetImage.sprite = greenSprite;
        targetImage.color = new Color(1f, 1f, 1f, 1f);
    }

    IEnumerator BlinkEffect()
    {
        while (true)
        {
            // Bright
            targetImage.color = new Color(1f, 1f, 1f, brightAlpha);
            yield return new WaitForSeconds(blinkSpeed);

            // Dim
            targetImage.color = new Color(1f, 1f, 1f, dimAlpha);
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}
