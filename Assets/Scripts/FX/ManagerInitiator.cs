using FX;
using Game.SO.EventChannel;
using NUnit.Framework;
using UnityEngine;
using Utility.VisualizableDictionary;

public class ManagerInitiator : MonoBehaviour
{

    [SerializeField] private PlaySFXEventChannelSO currentSFXSO;
    [SerializeField] private GenerateParticleEventChannelSO currentParticleSO;
    [SerializeField] private InstantiateGroupEventChannelSO currentInstantiateGroupSO;
    [SerializeField] private VisualizableDict<string, GameObject> currentInstantiateGroup;
    
    void Awake()
    {
        EffectManager.SetParticleEventChannel(currentParticleSO);
        EffectManager.SetSFXEventChannel(currentSFXSO);
        currentInstantiateGroupSO.Raise(currentInstantiateGroup.dict);
    }

    
    
}
