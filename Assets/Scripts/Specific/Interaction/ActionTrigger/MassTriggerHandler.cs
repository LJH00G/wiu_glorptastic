
using System;
using UnityEngine;
using Utility.VisualizableDictionary;


namespace Game.Interactable.MassTriggerHandler
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class MassTriggerHandler<T> : MonoBehaviour, I_Interactable
    {
        public abstract class TriggerableList<T_Triggerable>
        {
            [SerializeField] int index = 0;
            [SerializeField] T_Triggerable[] triggerables;
            /// <summary>if useExhaustedTriggerable, this will have no effect</summary>
            [Tooltip("if useExhaustedTriggerable, this will have no effect")]
            [SerializeField] bool cycleToStartWhenExhausted;
            [SerializeField] bool useExhaustedTriggerable;
            [SerializeField] T_Triggerable exhaustedTriggerable;

            public bool TryGetTriggerable(out T_Triggerable triggerable)
            {
                triggerable = exhaustedTriggerable;

                if (triggerables.Length == 0)
                    return false;

                if (index < 0 || index >= triggerables.Length)
                {
                    if (useExhaustedTriggerable)
                        return true;

                    index = cycleToStartWhenExhausted ?
                        0 : (triggerables.Length - 1);
                }

                triggerable = triggerables[index];
                
                index++;

                return true;
            }


#if UNITY_EDITOR
            public void OnValidate()
            {
                if (useExhaustedTriggerable && typeof(T_Triggerable).IsClass && exhaustedTriggerable == null)
                    Debug.LogError($"TriggerableList<T_Triggerable> | useExhaustedTriggerable == true, T_Triggerable is a class, exhaustedTriggerable cannot be left empty");
            }
#endif
        }

        [Serializable]
        public class TypedTriggerableList : TriggerableList<T>
        {

        }


        [Header("Triggerable List")]
        [SerializeField] TypedTriggerableList defaultTriggerList = new();
        [SerializeField] VisualizableDict<string, TypedTriggerableList> flagOverrideTriggerLists = new();

        [Header("Lock")]
        [SerializeField] float lockTimeAfterTrigger;
        [SerializeField, DisplayOnly] float lockTimer;

        [field: Header("Trigger")]
        [field: SerializeField]
        public bool RequiresInteraction { get; private set; } = true;
        [SerializeField] Vector2 offset;
        [SerializeField] Vector2 size = Vector2.one;
        [SerializeField, DisplayOnly] BoxCollider2D triggerCollider;

        public bool Locked { get => lockTimer > 0; }


        protected abstract void TriggerTriggerable(ref T triggerable);

        public void Interact()
        {
            if (RequiresInteraction)
                Trigger();
        }
        public void Trigger()
        {
            if (Locked)
                return;


            bool useFlagOverride = false;
            string flagOverrideKey = "";
            foreach (var entry in flagOverrideTriggerLists.dict)
            {
                if (GameManager.CurrentUserData.Flags[entry.Key])
                {
                    flagOverrideKey = entry.Key;
                    useFlagOverride = true;
                    break;
                }
            }


            if (useFlagOverride)
            {
                if (flagOverrideTriggerLists[flagOverrideKey].TryGetTriggerable(out T triggerable))
                {
                    TriggerTriggerable(ref triggerable);
                    lockTimer = lockTimeAfterTrigger;
                }
            }
            else if (defaultTriggerList.TryGetTriggerable(out T triggerable))
            {
                TriggerTriggerable(ref triggerable);
                lockTimer = lockTimeAfterTrigger;
            }

        }

        void Awake()
        {
            triggerCollider = GetComponent<BoxCollider2D>();
            triggerCollider.offset = offset;
            triggerCollider.size = size;
            triggerCollider.isTrigger = true;

            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }


        private void Update()
        {
            if (lockTimer >= 0)
                lockTimer -= Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("trigger entered this MassTriggerHandler");
            if (!RequiresInteraction)
                Trigger();
        }


#if UNITY_EDITOR

        protected virtual void OnValidate_Editor() {}

        private void OnValidate()
        {
            Awake();

            OnValidate_Editor();

            flagOverrideTriggerLists.OnValidate();

            foreach (var entry in flagOverrideTriggerLists.dict)
            {
                entry.Value.OnValidate();
                if (!GameManager.CurrentUserData.Flags.dict.TryGetValue(entry.Key, out bool _))
                {
                    Debug.LogError($"flagOverrideTriggerLists contains invalid flag", this);
                }
            }

            defaultTriggerList.OnValidate();

        }
#endif
    }
}