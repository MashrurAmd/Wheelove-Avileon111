using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageColorCycle : MonoBehaviour
{
    [Header("Image Settings")]
    public Image targetImage;
    public Sprite greenSprite;
    public Sprite redSprite;

    [Header("Timing Settings")]
    public float delayBeforeRed = 4f;
    public float redDuration = 10f;

    private void Start()
    {
        // Start with green image
        targetImage.sprite = greenSprite;

        // Start cycle
        StartCoroutine(ColorCycle());
    }

    IEnumerator ColorCycle()
    {
        // Wait before switching to red
        yield return new WaitForSeconds(delayBeforeRed);

        // Switch to red
        targetImage.sprite = redSprite;

        // Stay red for 10 seconds
        yield return new WaitForSeconds(redDuration);

        // Switch back to green
        targetImage.sprite = greenSprite;
    }
}
