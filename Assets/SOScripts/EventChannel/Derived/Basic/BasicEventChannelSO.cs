using UnityEngine;


namespace Game.SO.EventChannel
{
    [CreateAssetMenu(fileName = "BoolEvent_Channel", menuName = "Scriptable Objects/EventChannel/Basic/BoolEventChannelSO")]
    public class BoolEventChannelSO : EventChannelSO<bool>
    {

    }

    [CreateAssetMenu(fileName = "IntEvent_Channel", menuName = "Scriptable Objects/EventChannel/Basic/IntEventChannelSO")]
    public class IntEventChannelSO : EventChannelSO<int>
    {

    }

    [CreateAssetMenu(fileName = "ByteEvent_Channel", menuName = "Scriptable Objects/EventChannel/Basic/ByteEventChannelSO")]
    public class ByteEventChannelSO : EventChannelSO<byte>
    {

    }

    [CreateAssetMenu(fileName = "FloatEvent_Channel", menuName = "Scriptable Objects/EventChannel/Basic/FloatEventChannelSO")]
    public class FloatEventChannelSO : EventChannelSO<float>
    {

    }

    [CreateAssetMenu(fileName = "GameObjectEvent_Channel", menuName = "Scriptable Objects/EventChannel/Basic/GameObjectEventChannelSO")]
    public class GameObjectEventChannelSO : EventChannelSO<GameObject>
    {

    }
}