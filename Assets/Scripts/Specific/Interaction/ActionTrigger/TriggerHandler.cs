
using UnityEngine;


namespace Game.Interactable.TriggerHandler
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class TriggerHandler : MonoBehaviour, I_Interactable
    {
        [field: Header("Trigger")]
        [field: SerializeField]
        public bool RequiresInteraction { get; private set; } = true;
        [SerializeField] Vector2 offset;
        [SerializeField] Vector2 size = Vector2.one;
        [SerializeField, DisplayOnly] BoxCollider2D triggerCollider;

        [Header("Lock")]
        [SerializeField] float lockTimeAfterTrigger;
        [SerializeField, DisplayOnly] float lockTimer;

        public bool Locked { get => lockTimer > 0; }


        protected void ResetLockTimer()
        {
            lockTimer = lockTimeAfterTrigger;
        }


        public void Interact()
        {
            if (RequiresInteraction)
                Trigger();
        }
        public abstract void Trigger();

        protected void Awake()
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
            Debug.Log($"trigger entered this MassTriggerHandler {this.gameObject}" );
            if (!RequiresInteraction)
                Trigger();
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            Awake();

            if (TryGetComponent(out EntityOverworldController _))
                Debug.LogError("TriggerHandlers cannot be placed directly under an object with EntityOverworldController, please make a child and attach this script there intead", this);

        }
#endif
    }
}