using Game;
using Game.SO.EventChannel;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FlagEnabler : MonoBehaviour
{
    [Serializable]
    public struct FlagEnable
    {
        [Tooltip("enables this if flag is the value specified")]
        public GameObject gameObj;
        public string flagKey;
        public bool flagValue;
    }

    [SerializeField] List<FlagEnable> SetFlagList = new();


    private void Start()
    {
        foreach (var entry in SetFlagList)
        {
            GameManager.EnsureFlag(entry.flagKey);
        }
    }


    private void Update()
    {
        foreach (var entry in SetFlagList)
        {
            entry.gameObj.SetActive(
                GameManager.CurrentUserData.Flags[entry.flagKey] == entry.flagValue
                );
        }
    }

}
