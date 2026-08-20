using Game.SO.Behaviour.EntityOverworld;
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using UnityEngine;

public class EntityOverworldController : MonoBehaviour
{

    [Header("Behaviour")]
    [SerializeField] EntityOverworldBehaviourSO behaviour;
    [field: SerializeField]
    public EntityOverworldBehaviourInstanceData InstanceData { get; set; }


    public void SetBehaviour(EntityOverworldBehaviourSO behaviour)
    {
        this.behaviour = behaviour;
        this.behaviour.BehaviourStart(this);
    }


    void Start()
    {
        SetBehaviour(behaviour);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        behaviour.BehaviourUpdate(this, dt);
    }


#if UNITY_EDITOR

    EntityOverworldBehaviourSO prevBehaviour;
    private void OnValidate()
    {
        if (prevBehaviour != behaviour)
        {
            prevBehaviour = behaviour;
            SetBehaviour(behaviour);
        }
    }

    private void OnDrawGizmosSelected()
    {
        behaviour.BehaviourOnDrawGizmosSelected(this);
    }
#endif

}
