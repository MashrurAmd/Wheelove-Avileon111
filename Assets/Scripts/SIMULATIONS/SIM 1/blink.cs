using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GasButtonBlink : MonoBehaviour
{
    [Header("References")]
    public Image targetImage;          // real gas button image
    public Sprite greenSprite;         // normal
    public Sprite redSprite;           // stop
    public TriggerZone carTriggerZone; // ANY trigger zone

    [Header("Blink Settings")]
    public float blinkSpeed = 0.5f;
    public float dimAlpha = 0.3f;
    public float brightAlpha = 1f;
    public float blinkDuration = 5f;

    private Coroutine blinkRoutine;

    void Update()
    {
        if (carTriggerZone == null || targetImage == null)
            return;

        // 🚗 inside trigger zone
        if (carTriggerZone.isTriggered)
        {
            // set red sprite
            targetImage.sprite = redSprite;

            // start blinking once
            if (blinkRoutine == null)
            {
                blinkRoutine = StartCoroutine(BlinkRed());
            }
        }
        else
        {
            // 🚗 outside trigger zone
            if (blinkRoutine != null)
            {
                StopCoroutine(blinkRoutine);
                blinkRoutine = null;
            }

            // go back to green solid
            targetImage.sprite = greenSprite;
            targetImage.color = Color.white;
        }
    }

    IEnumerator BlinkRed()
    {
        float timer = 0f;

        while (timer < blinkDuration)
        {
            // bright red
            targetImage.color = new Color(1f, 1f, 1f, brightAlpha);
            yield return new WaitForSeconds(blinkSpeed);

            // dim red
            targetImage.color = new Color(1f, 1f, 1f, dimAlpha);
            yield return new WaitForSeconds(blinkSpeed);

            timer += blinkSpeed * 2f;
        }

        // after blinking → solid red
        blinkRoutine = null;
        targetImage.color = new Color(1f, 1f, 1f, brightAlpha);
    }
}
