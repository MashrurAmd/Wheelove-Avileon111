using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class TriggerZoneSpriteHandler : MonoBehaviour
{
    [Header("UI References")]
    public Image targetImage;
    public Sprite greenSprite;
    public Sprite middleSprite;    // 🟡 NEW: intermediate sprite
    public Sprite redSprite;

    [Header("Transition Settings")]
    public float middleSpriteDuration = 2f; // 🕒 stays for 2 seconds

    [Header("Blink Settings (Optional)")]
    public bool enableBlink = true;
    public float blinkSpeed = 0.5f;
    public float dimAlpha = 0.3f;
    public float brightAlpha = 1f;

    private Coroutine blinkRoutine;
    private Coroutine transitionRoutine;

    private void Awake()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        box.isTrigger = true;

        if (targetImage != null)
        {
            targetImage.sprite = greenSprite;
            targetImage.color = Color.white;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(GreenToRedSequence());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Stop all effects
        if (transitionRoutine != null)
        {
            StopCoroutine(transitionRoutine);
            transitionRoutine = null;
        }

        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        // Reset to green
        if (targetImage != null)
        {
            targetImage.color = Color.white;
            targetImage.sprite = greenSprite;
        }
    }

    private IEnumerator GreenToRedSequence()
    {
        // 🟡 Step 1: show middle sprite
        if (targetImage != null && middleSprite != null)
        {
            targetImage.color = Color.white;
            targetImage.sprite = middleSprite;
        }

        yield return new WaitForSeconds(middleSpriteDuration);

        // 🔴 Step 2: switch to red
        if (targetImage != null)
        {
            targetImage.sprite = redSprite;

            if (enableBlink)
                blinkRoutine = StartCoroutine(BlinkRed());
        }
    }

    private IEnumerator BlinkRed()
    {
        while (true)
        {
            targetImage.color = new Color(1f, 1f, 1f, brightAlpha);
            yield return new WaitForSeconds(blinkSpeed);

            targetImage.color = new Color(1f, 1f, 1f, dimAlpha);
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}
