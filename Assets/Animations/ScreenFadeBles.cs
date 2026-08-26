using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void FadeIn()
    {
        animator.SetTrigger("FadeIn");
    }
    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
    }
}