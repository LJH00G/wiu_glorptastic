
using UnityEngine;
using Utility.VisualizableDictionary;


namespace Game.Interactable.SingleTriggerHandler
{

    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class SingleTriggerHandler<T> : MonoBehaviour, I_Interactable
    {

        [Header("Triggerable")]
        [SerializeField] T triggerable;
        [SerializeField] VisualizableDict<string, T> flagOverrideTriggerables;

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

            lockTimer = lockTimeAfterTrigger;
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
            if (!RequiresInteraction)
                Trigger();
        }


#if UNITY_EDITOR

        protected virtual void OnValidate_Editor() { }


        private void OnValidate()
        {
            Awake();

            OnValidate_Editor();

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