using UnityEngine;
using UnityEngine.UI;
using Game.SO.EventChannel.Context;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Image fadeImage;
    [SerializeField] private FadeEventChannelSO onTPChannel;
    public void FadeIn(float timeTaken)
    {
        Debug.Log("fading in");
        animator.speed = timeTaken;
        animator.SetTrigger("FadeIn");
        BlockRaycast();
    }
    public void FadeOut(float timeTaken)
    {
        Debug.Log("fading out");
        animator.speed = timeTaken;
        animator.SetTrigger("FadeOut");
        UnblockRaycast();
    }

    public void BlockRaycast()
    {
        fadeImage.raycastTarget = true;
    }

    public void UnblockRaycast()
    {
        fadeImage.raycastTarget = false;
    }

    public void FadeDecider(FadeEventChannelContext context)
    {
        if (context.isFade)
            FadeIn(context.time);
        else 
            FadeOut(context.time);
    }

    private void OnEnable()
    {
        onTPChannel.Subscribe(FadeDecider);

    }

    private void OnDisable()
    {
        onTPChannel.Unsubscribe(FadeDecider);
    }
}