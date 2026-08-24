
using UnityEngine;
using Utility.VisualizableDictionary;


namespace Game.Interactable.TriggerHandler.Single
{

    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class SingleTriggerHandler<T> : TriggerHandler
    {
        [Header("Triggerable")]
        [SerializeField] T triggerable;
        [SerializeField] VisualizableDict<string, T> flagOverrideTriggerables = new();

        protected abstract void TriggerTriggerable(ref T triggerable);

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
                TriggerTriggerable(ref triggerable);
            }
            else
                TriggerTriggerable(ref triggerable);

            ResetLockTimer();
        }


#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            foreach (var entry in flagOverrideTriggerables.dict)
            {
                if (!GameManager.CurrentUserData.Flags.dict.TryGetValue(entry.Key, out bool _))
                {
                    Debug.LogError($"flagOverrideTriggerLists contains invalid flag", this);
                }
            }

        }
#endif
    }
}