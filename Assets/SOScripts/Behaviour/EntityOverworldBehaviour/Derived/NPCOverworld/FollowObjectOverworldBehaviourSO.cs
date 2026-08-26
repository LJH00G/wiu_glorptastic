
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using System;
using UnityEngine;


namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    [Serializable]
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
        [SerializeField] float followDistMult;
        [SerializeField] bool defaultTargetsPlayer;


        public override void BehaviourStart(EntityOverworldController controller)
        {
            if (controller.InstanceData is not FollowObjectOverworldBehaviourInstanceData)
                controller.InstanceData = new FollowObjectOverworldBehaviourInstanceData();

            controller.AIPath.orientation = Pathfinding.OrientationMode.YAxisForward;
            controller.AIPath.maxSpeed = Speed;
            controller.AIPath.maxAcceleration = Acceleration;
            controller.AIPath.pickNextWaypointDist = controller.Radius * 3 * followDistMult;
            controller.AIPath.slowdownDistance = controller.Radius * 4 * followDistMult;
            controller.AIPath.endReachedDistance = controller.Radius * 2 * followDistMult;

            if (defaultTargetsPlayer && controller.InstanceData != null)
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