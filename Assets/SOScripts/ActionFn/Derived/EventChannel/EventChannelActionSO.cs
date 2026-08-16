using UnityEngine;
using Game.SO.EventChannel;

namespace Game.SO.ActionFn
{
    public abstract class EventChannelActionSO<T_EventChannelSO, Context> : ActionSO
         where T_EventChannelSO : EventChannelSO<Context>
    {
        [SerializeField] T_EventChannelSO eventChannel;
        [SerializeField] Context context;

        public override void Invoke()
        {
            eventChannel.Raise(context);
        }
    }

    [CreateAssetMenu(fileName = "EventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/EventChannelActionSO")]
    public class EventChannelActionSO : ActionSO
    {
        [SerializeField] EventChannelSO eventChannel;

        public override void Invoke()
        {
            eventChannel.Raise();
        }
    }
}