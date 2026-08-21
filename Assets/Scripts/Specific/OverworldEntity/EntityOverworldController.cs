using Game;
using Game.SO.Behaviour.EntityOverworld;
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using Pathfinding;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AIPath))]
public class EntityOverworldController : MonoBehaviour
{

    [Header("Data")]
    [field: SerializeField]
    public float Radius { get; private set; } = 0.25f;

    [Header("Behaviour")]
    [SerializeField] EntityOverworldBehaviourSO behaviour;
    [field: SerializeField]
    public EntityOverworldBehaviourInstanceData InstanceData { get; set; }

    public AIPath AIPath { get; private set; }
    public BoxCollider2D triggerCollider { get; private set; }


    public void SetBehaviour(EntityOverworldBehaviourSO behaviour)
    {
        this.behaviour = behaviour;
        this.behaviour.BehaviourStart(this);
    }



    void Start()
    {
        AIPath = GetComponent<AIPath>();
        AIPath.radius = Radius;
        AIPath.gravity = Vector3.zero;
        AIPath.enableRotation = false;

        triggerCollider = GetComponent<BoxCollider2D>();
        triggerCollider.size = new Vector2(Radius * 2, Radius);
        triggerCollider.offset = new Vector2(0, Radius * 0.5f);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        SetBehaviour(behaviour);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (behaviour)
            behaviour.BehaviourUpdate(this, dt);

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

        triggerCollider = GetComponent<BoxCollider2D>();
        triggerCollider.size = new Vector2(Radius * 2, Radius);
        triggerCollider.offset = new Vector2(0, Radius * 0.5f);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (!behaviour)
            Debug.LogError("behaviour must not be left empty", this);

        if (prevBehaviour != behaviour)
        {
            prevBehaviour = behaviour;
            SetBehaviour(behaviour);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (behaviour)
            behaviour.BehaviourOnDrawGizmosSelected(this);
    }
#endif

}
