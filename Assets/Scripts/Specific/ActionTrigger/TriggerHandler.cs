
using UnityEngine;
using Utility.VisualizableDictionary;


namespace Game.TriggerHandler
{
    public abstract class TriggerableList<T_triggable>
        where T_triggable : class
    {
        public T_triggable[] triggables;
        /// <summary>if exhaustedAction exist, this will have no effect</summary>
        [Tooltip("if exhaustedTriggerable exist, this will have no effect")]
        public bool cycleToStartWhenExhausted;
        public T_triggable exhaustedTriggerable;

        protected abstract void Trigger(T_triggable triggerable);

        public void TriggerAt(ref int index)
        {
            if (triggables.Length == 0)
                return;

            if (index < 0 || index >= triggables.Length)
            {
                if (exhaustedTriggerable != null)
                {
                    Trigger(exhaustedTriggerable);
                    return;
                }

                index = cycleToStartWhenExhausted ?
                    0 : (triggables.Length - 1);
            }

            Trigger(triggables[index]);

            index++;
        }
    }


    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class TriggerHandler<T_TriggerableList, T_triggable> : MonoBehaviour, I_TriggerHandler
        where T_triggable : class
        where T_TriggerableList : TriggerableList<T_triggable>, new()
    {

        [Header("Triggerable List")]
        [SerializeField] int listIndexToBeTriggered = 0;
        [SerializeField] T_TriggerableList defaultTriggerList = new();
        [SerializeField] VisualizableDict<string, T_TriggerableList> flagOverrideTriggerLists = new();
        [SerializeField] float lockTimeAfterTrigger;
        [SerializeField, DisplayOnly] float lockTimer;

        [Header("Trigger")]
        [SerializeField] bool requiresInteraction = true;
        [SerializeField] Vector2 offset;
        [SerializeField] Vector2 size = Vector2.one;
        [SerializeField, DisplayOnly] BoxCollider2D triggerCollider;

        public bool Locked { get => lockTimer > 0; }


        public virtual void InitTriggerList(T_TriggerableList triggerlist) { }

        public bool RequiresInteraction()
        {
            return requiresInteraction;
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
                flagOverrideTriggerLists[flagOverrideKey].TriggerAt(ref listIndexToBeTriggered);
            else
                defaultTriggerList.TriggerAt(ref listIndexToBeTriggered);

            lockTimer = lockTimeAfterTrigger;
        }

        void Awake()
        {
            triggerCollider = GetComponent<BoxCollider2D>();
            triggerCollider.offset = offset;
            triggerCollider.size = size;
            triggerCollider.isTrigger = true;

            gameObject.layer = LayerMask.NameToLayer("Interactable");

            InitTriggerList(defaultTriggerList);
            foreach (var entry in flagOverrideTriggerLists.dict)
                InitTriggerList(entry.Value);
        }


        private void Update()
        {
            if (lockTimer >= 0)
                lockTimer -= Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!requiresInteraction)
                Trigger();
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            Awake();

            flagOverrideTriggerLists.OnValidate();

            foreach (var entry in flagOverrideTriggerLists.dict)
            {
                if (!GameManager.CurrentUserData.Flags.dict.TryGetValue(entry.Key, out bool flag))
                {
                    Debug.LogError($"flagOverrideTriggerLists contains invalid flag", this);
                }
            }

        }
#endif
    }
}