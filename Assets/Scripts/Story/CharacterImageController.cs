using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharacterImageController : MonoBehaviour
{
    public Image image;
    public Color shadowColor;
    public Color originalColor;

    public float jumpPower;
    public int jumpCount;
    public float jumpDuration;

    public float shakeDuration;
    public float shakeStrength;
    public int shakeVibrato;

    public void AdjustImageColor(Image notTalkingCharacterImage)
    {
        SetShadowImage(notTalkingCharacterImage);
        SetImageOriginalColor(image, originalColor);
    }

    public void SetImage(Sprite sp)
    {
        if (sp == null)
            return;

        image.sprite = sp;
    }

    private void SetShadowImage(Image image)
    {
        if (image.gameObject.activeSelf == false)
            return;

        image.color = shadowColor;
    }

    private void SetImageOriginalColor(Image image, Color originalColor)
    {
        image.color = originalColor;
    }

    public void JumpVertically()
    {
        image.transform.DOJump(transform.position, jumpPower, jumpCount, jumpDuration);
    }

    public void Shake()
    {
        image.transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, 90, false, true);
    }
}
