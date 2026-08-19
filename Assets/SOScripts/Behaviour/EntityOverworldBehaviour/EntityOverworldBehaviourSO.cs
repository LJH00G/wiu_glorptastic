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
        public abstract void BehaviourStart(EntityOverworldController controller);
        public abstract void BehaviourUpdate(EntityOverworldController controller, float dt);

#if UNITY_EDITOR
        public virtual void BehaviourOnDrawGizmosSelected(EntityOverworldController controller) { }
#endif
    }
}