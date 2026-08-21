using Game.SO.EventChannel;
using UnityEngine;


public class EventTestScript : MonoBehaviour
{
    [SerializeField] EventChannelSO eventChannel;

    public void Raise()
    {
        eventChannel.Raise();
    }
}

public abstract class EventTestScript<T_EventChannelSO, T_EventContext> : MonoBehaviour
    where T_EventChannelSO : EventChannelSO<T_EventContext>
{
    [SerializeField] T_EventChannelSO eventChannel;
    [SerializeField] T_EventContext eventContext;

    public void Raise()
    {
        eventChannel.Raise(eventContext);
    }
}