using Game.SO.EventChannel.Context;
using System;
using UnityEngine;
using Game.SO.EventChannel;



namespace Game.SO.EventChannel.Context
{
    [Serializable]
    public class FadeEventChannelContext
    {
        public bool isFade;
        public float time;

        public FadeEventChannelContext(bool fade, float time)
        {
            this.isFade = fade;
            this.time = time;
        }
    }
}


[CreateAssetMenu(fileName = "FadeEvent_Channel", menuName = "Scriptable Objects/EventChannel/Specific/FadeEventChannelSO")]
public class FadeEventChannelSO : EventChannelSO<FadeEventChannelContext>
{

}
