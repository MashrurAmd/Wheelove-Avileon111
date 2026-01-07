using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GasButtonBlink : MonoBehaviour
{
    [Header("References")]
    public Image targetImage;
    public Sprite greenSprite;
    public Sprite redSprite;
    public TriggerZone carTriggerZone;

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

        if (carTriggerZone.isTriggered)
        {
            // set sprite to RED
            targetImage.sprite = redSprite;

            // already blinking? do nothing
            if (blinkRoutine == null)
            {
                blinkRoutine = StartCoroutine(BlinkRed());
            }
        }
        else
        {
            // OUTSIDE AREA → reset to GREEN
            if (blinkRoutine != null)
            {
                StopCoroutine(blinkRoutine);
                blinkRoutine = null;
            }

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

        // stop blinking after duration
        blinkRoutine = null;

        // keep solid red
        targetImage.color = new Color(1f, 1f, 1f, brightAlpha);
    }
}
