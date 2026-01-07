using UnityEngine;
using UnityEngine.UI;
using System.Collections;


[RequireComponent(typeof(BoxCollider))]
public class TriggerZoneSpriteHandler : MonoBehaviour
{
    [Header("UI References")]
    public Image targetImage;      // The gas button image
    public Sprite greenSprite;     // Normal state
    public Sprite redSprite;       // When car is inside

    [Header("Blink Settings (Optional)")]
    public bool enableBlink = true;
    public float blinkSpeed = 0.5f;
    public float dimAlpha = 0.3f;
    public float brightAlpha = 1f;

    private bool isBlinking = false;
    private Coroutine blinkRoutine;

    private void Awake()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        box.isTrigger = true;

        if (targetImage != null)
            targetImage.sprite = greenSprite; // initial state
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Change to red immediately
            if (targetImage != null)
            {
                targetImage.sprite = redSprite;

                if (enableBlink && blinkRoutine == null)
                    blinkRoutine = StartCoroutine(BlinkRed());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Reset to green
            if (targetImage != null)
            {
                if (blinkRoutine != null)
                {
                    StopCoroutine(blinkRoutine);
                    blinkRoutine = null;
                }

                targetImage.color = Color.white;
                targetImage.sprite = greenSprite;
            }
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
