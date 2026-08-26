
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    public enum DIRECTION
    {
        DOWN,
        UP,
        RIGHT,
        LEFT
    }

    [Serializable]
    public abstract class EntityOverworldBehaviourInstanceData
    {
        public DIRECTION facingDire = DIRECTION.DOWN;

        public void SetFacingDire(Vector2 vec)
        {
            if (Mathf.Abs(vec.x) > Mathf.Abs(vec.y))
                facingDire = vec.x > 0 ? DIRECTION.RIGHT : DIRECTION.LEFT;
            else
                facingDire = vec.y > 0 ? DIRECTION.UP : DIRECTION.DOWN;
        }

        public Vector2 GetVector2Dire()
        {
            switch (facingDire)
            {
                default:
                case DIRECTION.DOWN:
                    return Vector2.down;
                case DIRECTION.UP:
                    return Vector2.up;
                case DIRECTION.RIGHT:
                    return Vector2.right;
                case DIRECTION.LEFT:
                    return Vector2.left;
            }
        }
    }
}

    namespace Game.SO.Behaviour.EntityOverworld
{
    public abstract class EntityOverworldBehaviourSO : ScriptableObject
    {
        [field: SerializeField]
        public float Speed { get; private set; } = 2;
        [field: SerializeField]
        public float Acceleration { get; private set; } = 10;

        public abstract void BehaviourStart(EntityOverworldController controller);
        public abstract void BehaviourUpdate(EntityOverworldController controller, float dt);


#if UNITY_EDITOR

        public virtual void BehaviourOnValidate(EntityOverworldController controller) { }
        public virtual void BehaviourOnDrawGizmosSelected(EntityOverworldController controller) { }
#endif
    }
}