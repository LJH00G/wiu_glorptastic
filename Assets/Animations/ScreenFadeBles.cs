using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Image fadeImage;

    public void FadeIn()
    {
        animator.SetTrigger("FadeIn");
    }
    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
    }

    // called via Animation Event on the FadeOut clip, once the screen is fully covered
    public void BlockRaycast()
    {
        fadeImage.raycastTarget = true;
    }

    // called via Animation Event on the FadeIn clip, once the screen is fully clear
    public void UnblockRaycast()
    {
        fadeImage.raycastTarget = false;
    }
}