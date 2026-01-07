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
    private bool isBlinking = false;
    private bool hasBlinkedThisEntry = false; // ⭐ KEY FIX

    void Update()
    {
        if (carTriggerZone.isTriggered)
        {
            targetImage.sprite = redSprite;

            if (!hasBlinkedThisEntry)
            {
                StartRedBlink();
            }
        }
        else
        {
            ResetState();
        }
    }

    void StartRedBlink()
    {
        isBlinking = true;
        hasBlinkedThisEntry = true;
        blinkRoutine = StartCoroutine(BlinkEffect());
    }

    void ResetState()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        isBlinking = false;
        hasBlinkedThisEntry = false;

        targetImage.sprite = greenSprite;
        targetImage.color = Color.white;
    }

    IEnumerator BlinkEffect()
    {
        float elapsed = 0f;

        while (elapsed < blinkDuration)
        {
            targetImage.color = new Color(1f, 1f, 1f, brightAlpha);
            yield return new WaitForSeconds(blinkSpeed);
            elapsed += blinkSpeed;

            targetImage.color = new Color(1f, 1f, 1f, dimAlpha);
            yield return new WaitForSeconds(blinkSpeed);
            elapsed += blinkSpeed;
        }

        // 🛑 HARD STOP after 5 seconds
        isBlinking = false;
        targetImage.color = new Color(1f, 1f, 1f, brightAlpha);
    }
}
