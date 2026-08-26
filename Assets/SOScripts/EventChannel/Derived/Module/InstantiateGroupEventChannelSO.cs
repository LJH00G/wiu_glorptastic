using Game.SO.EventChannel.Context;
using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Game.SO.EventChannel
{
    [CreateAssetMenu(fileName = "InstantiateGroupEvent_Channel", menuName = "Scriptable Objects/EventChannel/Module/InstantiateGroupEventChannelSO")]
    public class InstantiateGroupEventChannelSO : EventChannelSO<Dictionary<string, GameObject>>
    {
        
    }
}