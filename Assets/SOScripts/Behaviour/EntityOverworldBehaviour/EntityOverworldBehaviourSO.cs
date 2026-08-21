using UnityEngine;

namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    public abstract class EntityOverworldBehaviourInstanceData
    {

    }
}

    namespace Game.SO.Behaviour.EntityOverworld
{
    public abstract class EntityOverworldBehaviourSO : ScriptableObject
    {
        [SerializeField] protected float speed = 2;
        [SerializeField] protected float acceleration = 10;

        public abstract void BehaviourStart(EntityOverworldController controller);
        public abstract void BehaviourUpdate(EntityOverworldController controller, float dt);

#if UNITY_EDITOR
        public virtual void BehaviourOnDrawGizmosSelected(EntityOverworldController controller) { }
#endif
    }
}