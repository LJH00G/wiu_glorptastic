
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using UnityEngine;


namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    public class FollowObjectOverworldBehaviourInstanceData : EntityOverworldBehaviourInstanceData
    {
        public Transform targetForm;
    }
}


namespace Game.SO.Behaviour.EntityOverworld
{
    [CreateAssetMenu(fileName = "FollowObjectOverworld_Behaviour", menuName = "Scriptable Objects/Behaviour/EntityOverworld/NPCOverworld/FollowObjectOverworldBehaviourSO")]
    public class FollowObjectOverworldBehaviourSO : NPCOverworldBehaviourSO
    {
        [Header("Follow Object")]
        [SerializeField] float followDist;
        [SerializeField] bool defaultTargetsPlayer;


        public override void BehaviourStart(EntityOverworldController controller)
        {
            if (controller.InstanceData is not FollowObjectOverworldBehaviourInstanceData)
                controller.InstanceData = new FollowObjectOverworldBehaviourInstanceData();

            controller.AIPath.orientation = Pathfinding.OrientationMode.YAxisForward;
            controller.AIPath.maxSpeed = speed;
            controller.AIPath.maxAcceleration = acceleration;
            controller.AIPath.pickNextWaypointDist = followDist * 1.5f;
            controller.AIPath.slowdownDistance = followDist * 1.5f;
            controller.AIPath.endReachedDistance = followDist;

            if (defaultTargetsPlayer)
                ((FollowObjectOverworldBehaviourInstanceData)controller.InstanceData).targetForm = GameManager.Player.transform;
        }

        public override void BehaviourUpdate(EntityOverworldController controller, float dt)
        {
            if (controller.InstanceData is not FollowObjectOverworldBehaviourInstanceData instanceData)
                return;

            controller.AIPath.destination = instanceData.targetForm.position;
        }

    }
}