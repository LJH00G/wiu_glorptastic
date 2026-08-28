
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
            [SerializeField] public Triggerable[] triggerables;
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
                if (GameManager.CurrentUserData.Flags.dict.TryGetValue(entry.Key, out bool flagSet) && flagSet)
                {
                    Debug.Log($"entry.Key: {entry.Key}, value: {flagSet}", this);
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
                    GameManager.SetFlag(triggerable.flag.key, triggerable.flag.value);
                    ResetLockTimer();
                }
            }
            else if (defaultTriggerSequence.TryGetTriggerable(out Triggerable triggerable))
            {
                TriggerType(ref triggerable.type);
                GameManager.SetFlag(triggerable.flag.key, triggerable.flag.value);
                ResetLockTimer();
            }

        }


        private void Start()
        {
            foreach (var triggerable in defaultTriggerSequence.triggerables)
                GameManager.EnsureFlag(triggerable.flag.key, !triggerable.flag.value);

            flagOverrideTriggerSequences.OnValidate();

            foreach (var entry in flagOverrideTriggerSequences.dict)
                foreach (var triggerable in entry.Value.triggerables)
                    GameManager.EnsureFlag(triggerable.flag.key, !triggerable.flag.value);
        }



#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            flagOverrideTriggerSequences.OnValidate();

            foreach (var entry in flagOverrideTriggerSequences.dict)
                entry.Value.OnValidate();

            defaultTriggerSequence.OnValidate();

        }
#endif
    }
}