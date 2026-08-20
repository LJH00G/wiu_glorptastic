using Game.SO.EventChannel;
using UnityEngine;

namespace Game.SO.ActionFn.EventChannel
{
    [CreateAssetMenu(fileName = "GameObjectEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/Basic/GameObjectEventChannelActionSO")]
    public class GameObjectEventChannelActionSO : EventChannelActionSO<GameObjectEventChannelSO, GameObject>
    {

    }
}