using Game;
using Game.SO.EventChannel;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FlagSetter : MonoBehaviour
{
    [Serializable]
    public struct TriggerEventFlag
    {
        public EventChannelSO eventChannel;
        public string flagKey;
        public bool flagValue;

        public void SetFlag()
        {
            GameManager.SetFlag(flagKey, flagValue);
        }
    }

    [SerializeField] List<TriggerEventFlag> SetFlagList = new();


    private void Start()
    {
        foreach (var entry in SetFlagList)
        {
            GameManager.EnsureFlag(entry.flagKey);
        }
    }


    private void OnEnable()
    {
        foreach (var entry in SetFlagList)
        {
            entry.eventChannel.Subscribe(entry.SetFlag);
        }
    }

    private void OnDisable()
    {
        foreach (var entry in SetFlagList)
        {
            entry.eventChannel.Unsubscribe(entry.SetFlag);
        }
    }

}
