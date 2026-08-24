using Game;
using Game.SO.Behaviour.EntityOverworld;
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using Pathfinding;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(Animator))]
public class EntityOverworldController : MonoBehaviour
{

    [Header("Data")]
    [field: SerializeField]
    public float Radius { get; private set; } = 0.25f;

    [Header("Behaviour")]
    [SerializeField] EntityOverworldBehaviourSO behaviour;
    [field: SerializeReference]
    public EntityOverworldBehaviourInstanceData InstanceData { get; set; }

    public AIPath AIPath { get; private set; }
    public BoxCollider2D TriggerCollider { get; private set; }
    public Animator Animator { get; private set; }


    public void SetBehaviour(EntityOverworldBehaviourSO behaviour)
    {
        this.behaviour = behaviour;
        this.behaviour.BehaviourStart(this);
    }


    public void RefreshMovement()
    {
        AIPath.destination = transform.position;
    }


    void Start()
    {
        AIPath = GetComponent<AIPath>();
        AIPath.radius = Radius;
        AIPath.gravity = Vector3.zero;
        AIPath.enableRotation = false;

        TriggerCollider = GetComponent<BoxCollider2D>();
        TriggerCollider.size = new Vector2(Radius * 2, Radius);
        TriggerCollider.offset = new Vector2(0, Radius * 0.5f);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        Animator = GetComponent<Animator>();

        SetBehaviour(behaviour);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (behaviour)
        {
            behaviour.BehaviourUpdate(this, dt);
            behaviour.UpdateAnimator(this);
        }
        if (!GameManager.AllCanMove)
            AIPath.destination = transform.position;
    }


#if UNITY_EDITOR

    EntityOverworldBehaviourSO prevBehaviour;
    private void OnValidate()
    {
        AIPath = GetComponent<AIPath>();
        AIPath.radius = Radius;
        AIPath.gravity = Vector3.zero;
        AIPath.enableRotation = false;

        TriggerCollider = GetComponent<BoxCollider2D>();
        TriggerCollider.size = new Vector2(Radius * 2, Radius);
        TriggerCollider.offset = new Vector2(0, Radius * 0.5f);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        Animator = GetComponent<Animator>();

        if (behaviour)
        {
            if (prevBehaviour != behaviour) {
                prevBehaviour = behaviour;
                SetBehaviour(behaviour);
            }

            behaviour.BehaviourOnValidate(this);

            SpriteRenderer spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer)
                spriteRenderer.sprite = behaviour.DefaultSprite;
        }
        else
            Debug.LogError("behaviour must not be left empty", this);
    }

    private void OnDrawGizmosSelected()
    {
        if (behaviour)
            behaviour.BehaviourOnDrawGizmosSelected(this);
    }
#endif

}
