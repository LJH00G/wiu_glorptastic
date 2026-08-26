
using System;
using UnityEngine;
using Utility.VisualizableDictionary;


namespace Game.Interactable.TriggerHandler.Single
{

    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class SingleTriggerHandler<T> : TriggerHandler
    {
        [Serializable]
        public class Triggerable
        {
            public T type;
            public DictEntry<string, bool> flag;
        }

        [Header("Triggerable")]
        [SerializeField] Triggerable triggerable;
        [SerializeField] VisualizableDict<string, Triggerable> flagOverrideTriggerables = new();

        protected abstract void TriggerType(ref T type);

        public override void Trigger()
        {
            if (Locked)
                return;


            bool useFlagOverride = false;
            string flagOverrideKey = "";
            foreach (var entry in flagOverrideTriggerables.dict)
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
                var triggerable = flagOverrideTriggerables.dict[flagOverrideKey];
                TriggerType(ref triggerable.type);
                GameManager.CurrentUserData.Flags[triggerable.flag.key] = triggerable.flag.value;
            }
            else
            {
                TriggerType(ref triggerable.type);
                GameManager.CurrentUserData.Flags[triggerable.flag.key] = triggerable.flag.value;
            }

            ResetLockTimer();
        }


#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            if (!string.IsNullOrEmpty(triggerable.flag.key) && !GameManager.CurrentUserData.Flags.dict.ContainsKey(triggerable.flag.key))
                Debug.LogError($"triggerable contains invalid flag", this);

            foreach (var entry in flagOverrideTriggerables.dict)
            {
                if (!GameManager.CurrentUserData.Flags.dict.ContainsKey(entry.Key))
                {
                    Debug.LogError($"flagOverrideTriggerables contains invalid flag", this);
                }

                if (!string.IsNullOrEmpty(entry.Value.flag.key) && !GameManager.CurrentUserData.Flags.dict.ContainsKey(entry.Value.flag.key))
                {
                    Debug.LogError($"flagOverrideTriggerables contains entry that has value with invalid flag", this);
                }
            }

        }
#endif
    }
}