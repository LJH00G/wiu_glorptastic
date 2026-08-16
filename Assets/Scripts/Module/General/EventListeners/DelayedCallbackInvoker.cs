
using Game.SO.EventChannel.Context;
using Game.SO.EventChannel;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LightTransport;

public class DelayedCallbackInvoker : MonoBehaviour
{
    [Header("Event Listening Channel")]
    [SerializeField] DelayedCallbackEventChannelSO delayedCallbackEventChannel;

    [Serializable]
    public class DelayedCallback
    {
        public Action method;
        public float timer;
        public bool dtScaledOrUnscaled;

        public DelayedCallback(Action method, float delay, bool dtScaledOrUnscaled)
        {
            this.method = method;
            timer = delay;
            this.dtScaledOrUnscaled = dtScaledOrUnscaled;
        }
    }

    [SerializeField, DisplayOnly] List<DelayedCallback> delayedCallbacks = new();
    List<Action> iterationBuffer = new();
    bool locked = false;

    void HandleDelayedCallbackEvent(DelayedCallbackEventContext context)
    {
        if (context.addOrRemove)
            InvokeDelayed(context.method, context.delay, context.dtScaledOrUnscaled);
        else
            Withdraw(context.method);
    }

    /// <summary>
    /// use lambda for whatever method you put in that needs parameters <br/>
    /// eg.
    /// <code>
    /// InokeDelayed(
    ///     () => DoStuff(20),
    ///     1.5f
    /// );
    /// </code>
    /// </summary>
    public void InvokeDelayed(Action method, float delay, bool dtScaledOrUnscaled = true)
    {
        if (!locked)
            delayedCallbacks.Add(new DelayedCallback(method, delay, dtScaledOrUnscaled));
        else
            iterationBuffer.Add(
                () => delayedCallbacks.Add(new DelayedCallback(method, delay, dtScaledOrUnscaled))
                );
    }

    /// <summary>
    /// remove a method from the <see cref="delayedCallbacks"/>.
    /// </summary>
    public void Withdraw(Action method)
    {
        if (!locked)
            delayedCallbacks.RemoveAll(
                delayedCallback => delayedCallback.method == method
            );
        else
            iterationBuffer.Add(
                () => delayedCallbacks.RemoveAll(
                        delayedCallback => delayedCallback.method == method
                    )
                );
    }

    void Update()
    {
        float dt = Time.deltaTime;
        float unscaledDT = Time.unscaledDeltaTime;

        locked = true;
        foreach (var delayedCallback in delayedCallbacks)
        {
            if (delayedCallback.dtScaledOrUnscaled)
                delayedCallback.timer -= dt;
            else
                delayedCallback.timer -= unscaledDT;

            try
            {
                if (delayedCallback.timer <= 0)
                    delayedCallback.method?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

        }
        locked = false;

        delayedCallbacks.RemoveAll(
            delayedCallback => delayedCallback.timer <= 0
        );

        foreach (var buffer in iterationBuffer)
            buffer.Invoke();
        iterationBuffer.Clear();

    }

    private void OnEnable()
    {
        delayedCallbackEventChannel.Subscribe(HandleDelayedCallbackEvent);
    }

    private void OnDisable()
    {
        delayedCallbackEventChannel.Unsubscribe(HandleDelayedCallbackEvent);
    }

}
