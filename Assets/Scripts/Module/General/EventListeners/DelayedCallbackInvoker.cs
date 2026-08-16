
using Game.SO.EventChannel.Context;
using Game.SO.EventChannel.Derived;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DelayedCallbackInvoker : MonoBehaviour
{
    [Header("Event Listening Channel")]
    [SerializeField] DelayedCallbackEventChannelSO delayedCallbackEventChannel;

    [Serializable]
    public class DelayedCallback
    {
        public Action method;
        public float timer;

        public DelayedCallback(Action method, float delay)
        {
            this.method = method;
            timer = delay;
        }
    }

    [SerializeField, DisplayOnly] List<DelayedCallback> delayedCallbacks = new();
    List<Action> iterationBuffer = new();
    bool locked = false;

    void HandleDelayedCallbackEvent(DelayedCallbackEventContext context)
    {
        if (context.addOrRemove)
            InvokeDelayed(context.method, context.delay);
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
    public void InvokeDelayed(Action method, float delay)
    {
        if (!locked)
            delayedCallbacks.Add(new DelayedCallback(method, delay));
        else
            iterationBuffer.Add(
                () => delayedCallbacks.Add(new DelayedCallback(method, delay))
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
        float dt = Time.unscaledDeltaTime;

        locked = true;
        try
        {
            foreach (var delayedCallback in delayedCallbacks)
            {
                delayedCallback.timer -= dt;

                if (delayedCallback.timer <= 0)
                    delayedCallback.method?.Invoke();
            }
        }
        finally { locked = false; }

        delayedCallbacks.RemoveAll(
            delayedCallback => delayedCallback.timer <= 0
            );

        foreach (var buffer in iterationBuffer)
        {
            buffer.Invoke();
        }
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
