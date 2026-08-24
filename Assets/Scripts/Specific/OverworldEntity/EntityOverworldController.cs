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

    [field: Header("Data")]
    [field: SerializeField]
    public float Radius { get; private set; } = 0.25f;

    [field: Header("Appearance")]
    [field: SerializeField]
    public OverworldEntityAppearanceSO Appearance { get; set; }

    [field: Header("Behaviour")]
    [field: SerializeField]
    public EntityOverworldBehaviourSO Behaviour { get; set; }
    [field: SerializeReference]
    public EntityOverworldBehaviourInstanceData InstanceData { get; set; }

    public AIPath AIPath { get; private set; }
    public BoxCollider2D TriggerCollider { get; private set; }
    public Animator Animator { get; private set; }


    public void SetBehaviour(EntityOverworldBehaviourSO behaviour)
    {
        Behaviour = behaviour;
        Behaviour.BehaviourStart(this);
    }

    public void SetAppearance(OverworldEntityAppearanceSO appearance)
    {
        Appearance = appearance;
        Appearance.UpdateAppearance(this);
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
        TriggerCollider.isTrigger = true;
        TriggerCollider.size = new Vector2(Radius * 2, Radius);
        TriggerCollider.offset = new Vector2(0, Radius * 0.5f);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        Animator = GetComponent<Animator>();

        SetBehaviour(Behaviour);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (Behaviour)
        {
            Behaviour.BehaviourUpdate(this, dt);
        }
        if (!GameManager.AllCanMove)
            AIPath.destination = transform.position;

        if (Appearance)
            Appearance.UpdateAnimator(this);
    }


#if UNITY_EDITOR

    private void OnValidate()
    {
        AIPath = GetComponent<AIPath>();
        AIPath.radius = Radius;
        AIPath.gravity = Vector3.zero;
        AIPath.enableRotation = false;

        TriggerCollider = GetComponent<BoxCollider2D>();
        TriggerCollider.isTrigger = true;
        TriggerCollider.size = new Vector2(Radius * 2, Radius);
        TriggerCollider.offset = new Vector2(0, Radius * 0.5f);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        Animator = GetComponent<Animator>();

        if (Behaviour)
        {
            Behaviour.BehaviourOnValidate(this);
        }
        else
            Debug.LogError("behaviour must not be left empty", this);

        if (Appearance)
        {
            Appearance.UpdateAppearance(this);
        }
        else
            Debug.LogError("Appearance must not be left empty", this);

    }

    private void OnDrawGizmosSelected()
    {
        if (Behaviour)
            Behaviour.BehaviourOnDrawGizmosSelected(this);
    }
#endif

}
