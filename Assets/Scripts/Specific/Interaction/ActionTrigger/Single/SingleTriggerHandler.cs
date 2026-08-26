
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
                if (GameManager.CurrentUserData.Flags.dict.TryGetValue(entry.Key, out bool flagSet) && flagSet)
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
        }
#endif
    }
}