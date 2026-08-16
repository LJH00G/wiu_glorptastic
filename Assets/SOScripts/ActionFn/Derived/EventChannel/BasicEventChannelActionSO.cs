using UnityEngine;
using Game.SO.EventChannel.Derived.Basic;

namespace Game.SO.ActionFn.Derived
{
    [CreateAssetMenu(fileName = "BoolEventChannel_Act", menuName = "Scriptable Objects/ActionFn/BasicEventChannel/BoolEventChannelActionSO")]
    public class BoolEventChannelActionSO : EventChannelActionSO<BoolEventChannelSO, bool>
    {

    }

    [CreateAssetMenu(fileName = "IntEventChannel_Act", menuName = "Scriptable Objects/ActionFn/BasicEventChannel/IntEventChannelActionSO")]
    public class IntEventChannelActionSO : EventChannelActionSO<IntEventChannelSO, int>
    {

    }

    [CreateAssetMenu(fileName = "ByteEventChannel_Act", menuName = "Scriptable Objects/ActionFn/BasicEventChannel/ByteEventChannelActionSO")]
    public class ByteEventChannelActionSO : EventChannelActionSO<ByteEventChannelSO, byte>
    {

    }

    [CreateAssetMenu(fileName = "FloatEventChannel_Act", menuName = "Scriptable Objects/ActionFn/BasicEventChannel/FloatEventChannelActionSO")]
    public class FloatEventChannelActionSO : EventChannelActionSO<FloatEventChannelSO, float>
    {

    }

    [CreateAssetMenu(fileName = "GameObjectEventChannel_Act", menuName = "Scriptable Objects/ActionFn/BasicEventChannel/GameObjectEventChannelActionSO")]
    public class GameObjectEventChannelActionSO : EventChannelActionSO<GameObjectEventChannelSO, GameObject>
    {

    }

    
}