
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    [Serializable]
    public class EnemyOverworldBehaviourInstanceData : EntityOverworldBehaviourInstanceData
    {
        public Collider2D detectionTrigger;
        public ContactFilter2D contactFilter;
    }
}


namespace Game.SO.Behaviour.EntityOverworld
{
    [CreateAssetMenu(fileName = "EnemyOverworld_Behaviour", menuName = "Scriptable Objects/Behaviour/EntityOverworld/NPCOverworld/EnemyOverworldBehaviourSO")]
    public class EnemyOverworldBehaviourSO : NPCOverworldBehaviourSO
    {
        [Header("Enemy")]
        [SerializeField] bool useExternalTriggerDetection;
        [SerializeField] float detectionRange;

        public override void BehaviourStart(EntityOverworldController controller)
        {
            if (controller.InstanceData is not EnemyOverworldBehaviourInstanceData)
                controller.InstanceData = new EnemyOverworldBehaviourInstanceData();

            controller.AIPath.orientation = Pathfinding.OrientationMode.YAxisForward;
            controller.AIPath.maxSpeed = speed;
            controller.AIPath.maxAcceleration = acceleration;
            controller.AIPath.pickNextWaypointDist = controller.Radius * 3;
            controller.AIPath.slowdownDistance = controller.Radius * 4;
            controller.AIPath.endReachedDistance = controller.Radius;
            controller.AIPath.destination = controller.transform.position;

            EnemyOverworldBehaviourInstanceData instanceData = (EnemyOverworldBehaviourInstanceData)controller.InstanceData;
            instanceData.contactFilter.SetLayerMask(LayerMask.GetMask("Player"));
            instanceData.contactFilter.useTriggers = true;
        }

        public override void BehaviourUpdate(EntityOverworldController controller, float dt)
        {
            if (controller.InstanceData is not EnemyOverworldBehaviourInstanceData instanceData)
                return;

            if (!useExternalTriggerDetection)
            {
                if ((GameManager.Player.transform.position - controller.transform.position).sqrMagnitude <= detectionRange * detectionRange)
                    controller.AIPath.destination = GameManager.Player.transform.position;

                return;
            }


            Collider2D[] colliders = new Collider2D[1];
            instanceData.detectionTrigger.Overlap(instanceData.contactFilter, colliders);

            var collider = colliders[0];

            if (collider && collider.attachedRigidbody.gameObject == GameManager.Player)
            {
                controller.AIPath.destination = GameManager.Player.transform.position;
                return;
            }
        }

        

#if UNITY_EDITOR
        public override void BehaviourOnDrawGizmosSelected(EntityOverworldController controller)
        {
            if (useExternalTriggerDetection || controller.InstanceData is not EnemyOverworldBehaviourInstanceData instanceData)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(controller.transform.position, detectionRange);

        }
#endif
    }
}