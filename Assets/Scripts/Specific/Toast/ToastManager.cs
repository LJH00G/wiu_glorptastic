using Game.SO.EventChannel.Context;
using Game.TextMarkup;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility.Math;

public class ToastManager : MonoBehaviour
{
    [Serializable]
    public struct Toast
    {
        public RectTransform rectForm;
        public CanvasGroup cGroup;
        public float dissappearTimer;
        public float dissappearAlpha;
        public float positionAlpha;
    }

    [Header("Event Listening Channel")]
    [SerializeField] ToastEventChannelSO toastEventChannel;

    [Header("Toast")]
    [SerializeField] Transform toastContainer;
    [SerializeField] GameObject toastPrefab;
    [SerializeField] float disappearTime;
    [SerializeField] float disappearFadeStartTime;
    float disappearFadeTime_inv;
    [SerializeField] int alphaFadeStartIndex;
    [SerializeField] int alphaFadeEndIndex;
    [SerializeField] float smoothFactor_inv;
    float alphaFadeTotalIndex_inv;

    [Header("Offsets")]
    [SerializeField] Vector2 offset;
    [SerializeField] float gap;

    [Header("Buffer")]
    [SerializeField] float bufferTime;
    [SerializeField, DisplayOnly] float bufferTimer;

    [Header("Dont Touch")]
    [SerializeReference] List<ToastEventContext> bufferedToast = new();
    [SerializeField] List<Toast> managedToast = new();

    public void AppendToast(ToastEventContext context)
    {
        bufferedToast.Add(context);
    }

    public void GenerateToast(ToastEventContext context)
    {
        GameObject instantiatedToast = Instantiate(toastPrefab, toastContainer);
        RectTransform toastRectForm = (RectTransform)instantiatedToast.transform;

        
        Image divider = toastRectForm.GetChild(0).GetComponent<Image>();
        var dividerColor = context.color;
        dividerColor.a = divider.color.a;
        divider.color = dividerColor;


        TextMarkupTypeWriter typeWriter = toastRectForm.GetChild(1).GetComponent<TextMarkupTypeWriter>();
        typeWriter.SetDefaultEffect(
            new()
            {
                new ColorTextMarkupEffect
                { 
                    color = context.color,
                    fadeColor = context.color
                }
            }
            );
        string msg = context.GetToastMessage();
        typeWriter.StartNewTypeWriting(msg, true);
        float width = typeWriter.TMPText.preferredWidth;


        Transform itemIconForm = toastRectForm.GetChild(2);
        Sprite sprite = null;

        switch (context)
        {
            case ItemStackToastEventContext itemStackToast:
                sprite = itemStackToast.itemStack.item.Sprite;
                break;
            case MessageToastEventContext messageToast:
                sprite = messageToast.sprite;
                break;
            default:
                break;
        }

        if (sprite)
        {
            Image itemIcon = itemIconForm.GetComponent<Image>();
            itemIcon.sprite = sprite;
            width += 35; // icon size
        }
        else
            itemIconForm.gameObject.SetActive(false);


        width += 50; // offset width from text
        toastRectForm.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            width
        );


        var position = toastRectForm.anchoredPosition;
        position.x += width + 50;
        toastRectForm.anchoredPosition = position;


        // type write again to fix offset issues
        typeWriter.StartNewTypeWriting(msg, true);

        managedToast.Insert(
            0,
            new()
            {
                rectForm = toastRectForm,
                cGroup = toastRectForm.GetComponent<CanvasGroup>(),
                dissappearTimer = 0,
                dissappearAlpha = 1,
                positionAlpha = 1,
            }
            );
    }

    private void Awake()
    {
        alphaFadeTotalIndex_inv = 1f / (alphaFadeEndIndex - alphaFadeStartIndex);
        disappearFadeTime_inv = 1f / (disappearTime - disappearFadeStartTime);
    }


    private void Update()
    {
        float dt = Time.unscaledDeltaTime;


        if (bufferTimer > 0)
            bufferTimer -= dt;
        else if (bufferedToast.Count > 0)
        {
            bufferTimer += bufferTime;
            GenerateToast(bufferedToast[0]);
            bufferedToast.RemoveAt(0);
        }

        for (int i = managedToast.Count - 1; i >= 0; i--)
        {
            var toast = managedToast[i];


            // move toast
            toast.rectForm.anchoredPosition =
                Vector2.Lerp(
                    toast.rectForm.anchoredPosition,
                    new Vector2(-offset.x, offset.y + i * (toast.rectForm.sizeDelta.y + gap)),
                    dt * smoothFactor_inv
                    );


            // disappear timer
            toast.dissappearTimer += dt;
            if (toast.dissappearTimer > disappearFadeStartTime)
                toast.dissappearAlpha =
                    Mathf.Lerp(
                        1,
                        0,
                        Math_Ease.Ease(EASE.OUT_QUAD, (toast.dissappearTimer - disappearFadeStartTime) * disappearFadeTime_inv)
                    );

            // remove when disappeared
            if (toast.dissappearTimer > disappearTime)
            {
                Destroy(toast.rectForm.gameObject);
                managedToast.RemoveAt(i);
                continue;
            }


            // lower alpha at larger index
            if (i < alphaFadeStartIndex)
            {
                UpdateToast(ref toast, i);
                continue;
            }

            toast.positionAlpha =
                Mathf.Lerp(
                    toast.positionAlpha,
                    1 - (i - alphaFadeStartIndex) * alphaFadeTotalIndex_inv,
                    dt * smoothFactor_inv
                    );


            // remove when alpha is 0
            if (i < alphaFadeEndIndex || !Math_F.Equal_3d(toast.positionAlpha, 0))
            {
                UpdateToast(ref toast, i);
                continue;
            }

            Destroy(toast.rectForm.gameObject);
            managedToast.RemoveAt(i);

        }

        void UpdateToast(ref Toast currentToast, int index)
        {
            currentToast.cGroup.alpha = currentToast.dissappearAlpha * currentToast.positionAlpha;
            managedToast[index] = currentToast;
        }
    }


    private void OnEnable()
    {
        toastEventChannel.Subscribe(AppendToast);
    }

    private void OnDisable()
    {
        toastEventChannel.Unsubscribe(AppendToast);
    }

}
