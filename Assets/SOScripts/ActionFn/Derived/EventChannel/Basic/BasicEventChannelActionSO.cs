using Game.SO.EventChannel;
using UnityEngine;

namespace Game.SO.ActionFn.EventChannel
{
    [CreateAssetMenu(fileName = "BoolEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/Basic/BoolEventChannelActionSO")]
    public class BoolEventChannelActionSO : EventChannelActionSO<BoolEventChannelSO, bool>
    {

    }

    [CreateAssetMenu(fileName = "IntEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/Basic/IntEventChannelActionSO")]
    public class IntEventChannelActionSO : EventChannelActionSO<IntEventChannelSO, int>
    {

    }

    [CreateAssetMenu(fileName = "ByteEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/Basic/ByteEventChannelActionSO")]
    public class ByteEventChannelActionSO : EventChannelActionSO<ByteEventChannelSO, byte>
    {

    }

    [CreateAssetMenu(fileName = "FloatEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/Basic/FloatEventChannelActionSO")]
    public class FloatEventChannelActionSO : EventChannelActionSO<FloatEventChannelSO, float>
    {

    }

    [CreateAssetMenu(fileName = "GameObjectEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/Basic/GameObjectEventChannelActionSO")]
    public class GameObjectEventChannelActionSO : EventChannelActionSO<GameObjectEventChannelSO, GameObject>
    {

    }
}