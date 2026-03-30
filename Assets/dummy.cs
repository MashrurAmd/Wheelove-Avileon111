using UnityEngine;
using UnityEngine.UI;

public class ImageSwapper : MonoBehaviour
{
    public Image targetImage;
    public Sprite imageA;
    public Sprite imageB;

    private bool isA = true;

    void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        targetImage.sprite = imageA;
    }

    public void SwapImage()
    {
        isA = !isA;
        targetImage.sprite = isA ? imageA : imageB;
    }
}