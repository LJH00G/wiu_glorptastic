using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;
using UnityEngine;

public class EventTestScript : MonoBehaviour
{
    [SerializeField] PlayMusicEventChannelSO playMusicEventChannel;
    [SerializeField] PlayMusicEventContext playMusicEventContext;

    public void Trigger()
    {
        playMusicEventChannel.Raise(playMusicEventContext);
    }
}
