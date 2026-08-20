using Game;
using Game.SO.Behaviour.EntityOverworld;
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using Pathfinding;
using UnityEngine;


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
