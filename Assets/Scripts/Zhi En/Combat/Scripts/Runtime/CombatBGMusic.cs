using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;
using UnityEngine;

public class CombatBGMusic : MonoBehaviour
{
    [SerializeField] PlayMusicEventChannelSO playMusicEventChannelSO;
    [SerializeField] PlayMusicEventContext playMusicEventContext;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playMusicEventChannelSO.Raise(playMusicEventContext);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
