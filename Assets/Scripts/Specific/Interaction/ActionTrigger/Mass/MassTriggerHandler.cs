
using System;
using UnityEngine;
using Utility.VisualizableDictionary;


namespace Game.Interactable.TriggerHandler.Mass
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class MassTriggerHandler<T> : TriggerHandler
    {
        [Serializable]
        public class Triggerable
        {
            public T type;
            public DictEntry<string, bool> flag;
        }

        [Serializable]
        public class TriggerableSequence
        {
            [SerializeField] int index = 0;
            [SerializeField] Triggerable[] triggerables;
            /// <summary>if useExhaustedTriggerable, this will have no effect</summary>
            [Tooltip("if useExhaustedTriggerable, this will have no effect")]
            [SerializeField] bool cycleToStartWhenExhausted;
            [SerializeField] bool useExhaustedTriggerable;
            [SerializeField] Triggerable exhaustedTriggerable;

            public bool TryGetTriggerable(out Triggerable triggerable)
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
                for (int i = 0; i < triggerables.Length; i++)
                {
                    var triggerable = triggerables[i];
                    if (!string.IsNullOrEmpty(triggerable.flag.key) && GameManager.CurrentUserData.Flags.dict.ContainsKey(triggerable.flag.key))
                        Debug.LogError($"TriggerableList<Triggerable<{typeof(T)}>> | triggerables contains entry that has value with invalid flag");
                }

                if (useExhaustedTriggerable && typeof(Triggerable).IsClass && exhaustedTriggerable == null)
                    Debug.LogError($"TriggerableList<Triggerable<{typeof(T)}>> | useExhaustedTriggerable == true, T_Triggerable is a class, exhaustedTriggerable cannot be left empty");
            }
#endif
        }


        [Header("Triggerable Sequence")]
        [SerializeField] TriggerableSequence defaultTriggerSequence = new();
        [SerializeField] VisualizableDict<string, TriggerableSequence> flagOverrideTriggerSequences = new();

        protected abstract void TriggerType(ref T triggerable);

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
                if (flagOverrideTriggerSequences[flagOverrideKey].TryGetTriggerable(out Triggerable triggerable))
                {
                    TriggerType(ref triggerable.type);
                    GameManager.CurrentUserData.Flags[triggerable.flag.key] = triggerable.flag.value;
                    ResetLockTimer();
                }
            }
            else if (defaultTriggerSequence.TryGetTriggerable(out Triggerable triggerable))
            {
                TriggerType(ref triggerable.type);
                GameManager.CurrentUserData.Flags[triggerable.flag.key] = triggerable.flag.value;
                ResetLockTimer();
            }

        }




#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            flagOverrideTriggerSequences.OnValidate();

            Debug.Log($"validating flagOverrideTriggerSequences", this);
            foreach (var entry in flagOverrideTriggerSequences.dict)
            {
                entry.Value.OnValidate();
                if (!GameManager.CurrentUserData.Flags.dict.ContainsKey(entry.Key))
                {
                    Debug.LogError($"flagOverrideTriggerSequences contains invalid flag", this);
                }
            }

            Debug.Log($"validating defaultTriggerSequence", this);
            defaultTriggerSequence.OnValidate();

        }
#endif
    }
}