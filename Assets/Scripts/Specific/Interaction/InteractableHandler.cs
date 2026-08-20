using Game;
using Game.SO.ActionFn;
using System;
using UnityEngine;
using Utility.VisualizableDictionary;

[RequireComponent(typeof(BoxCollider2D))]
public class ActionTriggerHandler : MonoBehaviour
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

    [Header("Triggering Action")]
    [SerializeField] int actionIndexToBeTriggered = 0;
    [SerializeField] Interaction defaultInteraction;
    [SerializeField] VisualizableDict<string, Interaction> flagOverrideInteractions;

    [Header("Trigger")]
    [field: SerializeField]
    public bool RequiresInteraction { get; private set; }
    [SerializeField] Vector2 offset;
    [SerializeField] Vector2 size;
    [SerializeField, DisplayOnly] BoxCollider2D actionTriggerCollider;

    void Awake()
    {
        actionTriggerCollider = GetComponent<BoxCollider2D>();
        actionTriggerCollider.offset = offset;
        actionTriggerCollider.size = size;
        actionTriggerCollider.isTrigger = true;

        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }


    public void TriggerInteraction()
    {
        bool useFlagOverride = false;
        string flagOverrideKey = "";
        foreach (var entry in flagOverrideInteractions.dict)
        {
            if (GameManager.CurrentUserData.Flags[entry.Key])
            {
                flagOverrideKey = entry.Key;
                useFlagOverride = true;
                break;
            }
        }

        if (useFlagOverride)
            flagOverrideInteractions[flagOverrideKey].Trigger(ref actionIndexToBeTriggered);
        else
            defaultInteraction.Trigger(ref actionIndexToBeTriggered);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!RequiresInteraction)
            TriggerInteraction();
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        Awake();

        flagOverrideInteractions.OnValidate();

        foreach (var entry in flagOverrideInteractions.dict)
        {
            if (!GameManager.CurrentUserData.Flags.dict.TryGetValue(entry.Key, out bool flag))
            {
                Debug.LogError($"InteractableHandler.OnValidate() | flagOverrideInteractions contains invalid flag", this);
            }
        }

    }
#endif
}
