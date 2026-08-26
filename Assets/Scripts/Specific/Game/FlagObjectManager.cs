using Game;
using UnityEngine;
using Utility.VisualizableDictionary;
using System.Collections.Generic;

public class FlagObjectManager : MonoBehaviour
{
    public enum FlagAction
    {
        Remove,
        Add
    }
    [System.Serializable]
    public class Flag
    {
        public string name;
        public bool status;
        public bool answered;
        public FlagAction action;
        public Transform transform;

        public Flag(string name = "Uninitialized", bool status = false, bool answered = false, FlagAction action = FlagAction.Add, Transform transform = null)
        {
            this.name = name;
            this.status = status;
            this.answered = answered;
            this.action = action;
            this.transform = transform;
        }
    }

    public VisualizableDict<Flag, GameObject> flagObjectDictionary;
    private readonly Dictionary<Flag, GameObject> activeInstances = new Dictionary<Flag, GameObject>();

    void Update()
    {
        List<Flag> triggeredFlags = CheckForFlags();

        foreach (Flag triggeredFlag in triggeredFlags)
        {
            if (triggeredFlag.name == "Uninitialized")
            {
                Debug.LogError($"Flag in FlagObjectManager is uninitialized: {triggeredFlag}");
                triggeredFlag.answered = true;
                continue; 
            }

            if (!flagObjectDictionary.dict.TryGetValue(triggeredFlag, out GameObject obj))
            {
                Debug.LogError($"Triggered Flag does not have a dictionary definition or object: {triggeredFlag}");
                triggeredFlag.answered = true;
                continue; 
            }

            if (triggeredFlag.action == FlagAction.Add)
            {
                
                Transform parent = triggeredFlag.transform != null ? triggeredFlag.transform : transform;
                GameObject instance = Instantiate(obj, parent);

                
                activeInstances[triggeredFlag] = instance;

                triggeredFlag.answered = true;
                continue;
            }

            
            if (activeInstances.TryGetValue(triggeredFlag, out GameObject instanceToRemove) && instanceToRemove != null)
            {
                
                if (instanceToRemove.scene.IsValid())
                {
                    Destroy(instanceToRemove);
                    activeInstances.Remove(triggeredFlag);
                    triggeredFlag.answered = true;
                }
                else
                {
                    Debug.LogError($"Tracked object for Flag is a Prefab asset, not a scene instance - refusing to destroy: {triggeredFlag} , {instanceToRemove}");
                    triggeredFlag.answered = true;
                }
            }
            else
            {
                Debug.LogError($"Tried to remove a Flag object with no tracked instance (never added, or already removed): {triggeredFlag} , {obj}");
                triggeredFlag.answered = true;
            }
        }
    }

    List<Flag> CheckForFlags()
    {
        
        List<Flag> flagsTriggered = new List<Flag>();

        foreach (Flag flagDef in flagObjectDictionary.dict.Keys)
        {
            if (!GameManager.CurrentUserData.Flags.dict.TryGetValue(flagDef.name, out bool status))
                continue;

            if (status && !flagDef.answered)
                flagsTriggered.Add(flagDef);
        }

        return flagsTriggered;
    }
}