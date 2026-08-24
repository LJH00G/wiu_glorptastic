
using System;
using UnityEngine;
using Utility.VisualizableDictionary;


namespace Game.Interactable.TriggerHandler.Mass
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class MassTriggerHandler<T> : TriggerHandler
    {
        public abstract class TriggerableSequence<T_Triggerable>
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
        public class TypedTriggerableSequence : TriggerableSequence<T>
        {

        }


        [Header("Triggerable Sequence")]
        [SerializeField] TypedTriggerableSequence defaultTriggerSequence = new();
        [SerializeField] VisualizableDict<string, TypedTriggerableSequence> flagOverrideTriggerSequences = new();

        protected abstract void TriggerTriggerable(ref T triggerable);

        public override void Trigger()
        {
            if (Locked)
                return;


            bool useFlagOverride = false;
            string flagOverrideKey = "";
            foreach (var entry in flagOverrideTriggerSequences.dict)
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
                if (flagOverrideTriggerSequences[flagOverrideKey].TryGetTriggerable(out T triggerable))
                {
                    TriggerTriggerable(ref triggerable);
                    ResetLockTimer();
                }
            }
            else if (defaultTriggerSequence.TryGetTriggerable(out T triggerable))
            {
                TriggerTriggerable(ref triggerable);
                ResetLockTimer();
            }

        }




#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            flagOverrideTriggerSequences.OnValidate();

            foreach (var entry in flagOverrideTriggerSequences.dict)
            {
                entry.Value.OnValidate();
                if (!GameManager.CurrentUserData.Flags.dict.TryGetValue(entry.Key, out bool _))
                {
                    Debug.LogError($"flagOverrideTriggerSequences contains invalid flag", this);
                }
            }

            defaultTriggerSequence.OnValidate();

        }
#endif
    }
}