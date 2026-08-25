
using System;
using UnityEngine;

namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    [Serializable]
    public abstract class EntityOverworldBehaviourInstanceData
    {
        public Vector2 facingDire = Vector2.down;
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

        public virtual void BehaviourOnValidate(EntityOverworldController controller) { }
        public virtual void BehaviourOnDrawGizmosSelected(EntityOverworldController controller) { }
#endif
    }
}