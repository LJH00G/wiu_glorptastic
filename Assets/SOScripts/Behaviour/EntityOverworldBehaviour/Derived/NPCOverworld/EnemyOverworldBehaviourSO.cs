
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using UnityEngine;


namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    public class EnemyOverworldBehaviourInstanceData : EntityOverworldBehaviourInstanceData
    {
        [SerializeField] Collider2D detectionTrigger;
        [SerializeField] int battleData; // replace with battle data when its done
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

            controller.Animator.runtimeAnimatorController = animCtrller;

        }

        public override void BehaviourUpdate(EntityOverworldController controller, float dt)
        {
            throw new System.NotImplementedException();
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