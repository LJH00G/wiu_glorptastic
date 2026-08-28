
using Game.SO.EventChannel;
using UnityEngine;
using Utility.VisualizableDictionary;

public class ManagerInitiator : MonoBehaviour
{

    [SerializeField] private InstantiateGroupEventChannelSO currentInstantiateGroupSO;
    [SerializeField] private VisualizableDict<string, GameObject> currentInstantiateGroup;
    
    void Awake()
    {
        currentInstantiateGroup.OnValidate();
        currentInstantiateGroupSO.Raise(currentInstantiateGroup.dict);
    }

    
    
}
