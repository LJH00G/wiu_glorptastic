
using System;
using UnityEngine;

namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    [Serializable]
    public abstract class EntityOverworldBehaviourInstanceData
    {
        public Vector2 facingDire;
    }
}

    namespace Game.SO.Behaviour.EntityOverworld
{
    public abstract class EntityOverworldBehaviourSO : ScriptableObject
    {
        [SerializeField] protected RuntimeAnimatorController animCtrller;
        [field: SerializeField]
        public Sprite DefaultSprite { get; private set; }
        [SerializeField] protected float speed = 2;
        [SerializeField] protected float acceleration = 10;

        public abstract void BehaviourStart(EntityOverworldController controller);
        public abstract void BehaviourUpdate(EntityOverworldController controller, float dt);

        public void UpdateAnimator(EntityOverworldController controller)
        {
            var speed = controller.AIPath.desiredVelocity.magnitude;

            if (speed > 0.1f)
            {

                float theta = Vector2.SignedAngle(Vector2.right, controller.AIPath.desiredVelocity);

                if (Mathf.Abs(theta) < 44)
                {
                    controller.Animator.SetInteger("Direction", 0);
                    controller.InstanceData.facingDire = Vector2.right;
                }
                else if (theta < 136 && theta > 44)
                {
                    controller.Animator.SetInteger("Direction", 1);
                    controller.InstanceData.facingDire = Vector2.up;
                }
                else if (Mathf.Abs(theta) > 136)
                {
                    controller.Animator.SetInteger("Direction", 2);
                    controller.InstanceData.facingDire = Vector2.left;
                }
                else
                {
                    controller.Animator.SetInteger("Direction", 3);
                    controller.InstanceData.facingDire = Vector2.down;
                }
            }


            controller.Animator.SetBool("Walking", !controller.AIPath.reachedEndOfPath);
            controller.Animator.SetFloat("Speed", speed * 0.3f);

        }


#if UNITY_EDITOR

        public virtual void OnValidate()
        {
            if (!animCtrller)
                Debug.LogError("animCtrller cannot be left enpty", this);
        }

        public virtual void BehaviourOnValidate(EntityOverworldController controller) { }
        public virtual void BehaviourOnDrawGizmosSelected(EntityOverworldController controller) { }
#endif
    }
}