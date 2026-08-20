using Game.SO.ActionFn;
using Utility.VisualizableDictionary;
using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractableHandler : MonoBehaviour
{
    [Serializable]
    public struct Interaction
    {
        public ActionSO[] actions;
        public ActionSO exhaustedAction;

        public void Trigger(ref int index)
        {
            if (index < 0 || index >= actions.Length)
            {
                exhaustedAction.Invoke();
                return;
            }

            actions[index].Invoke();
        }
    }

    [Header("Interaction")]
    [SerializeField] int actionIndexToBeTriggered = 0;
    [SerializeField] Interaction defaultInteraction;
    [SerializeField] VisualizableDict<string, Interaction> flagOverrideInteractions;

    [Header("Interaction Trigger")]
    [SerializeField] Vector2 offset;
    [SerializeField] Vector2 size;
    [SerializeField, DisplayOnly] BoxCollider2D interactableCollider;

    void Awake()
    {
        interactableCollider = GetComponent<BoxCollider2D>();
        interactableCollider.offset = offset;
        interactableCollider.size = size;
        interactableCollider.isTrigger = true;

        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }


    public void TriggerInteraction()
    {
        bool useFlagOverride = false;
        string flagOverrideKey = "";
        foreach (var entry in flagOverrideInteractions.dict)
        {
            // if (GameManager.UserData.Flags[entry.key])
            //{
            //flagOverrideKey = entry.key;
            //useFlagOverride = true;
            //break;
            //}
        }

        if (useFlagOverride)
            flagOverrideInteractions[flagOverrideKey].Trigger(ref actionIndexToBeTriggered);
        else
            defaultInteraction.Trigger(ref actionIndexToBeTriggered);
    }


    private void Update()
    {
        
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        Awake();

        flagOverrideInteractions.OnValidate();

        foreach (var entry in flagOverrideInteractions.dict)
        {
            //if (!GameManager.UserData.Flags.dict.TryGetValue(entry.key, out bool flag))
            //{
                //Debug.LogError($"InteractableHandler.OnValidate() | flagOverrideInteractions contains invalid flag", this);
            //}
        }

    }
#endif
}
