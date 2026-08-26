
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using System;
using UnityEngine;


namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    [Serializable]
    public class StaticOverworldBehaviourInstanceData : EntityOverworldBehaviourInstanceData
    {
        public DIRECTION defaultDirection;
    }
}


namespace Game.SO.Behaviour.EntityOverworld
{
    [CreateAssetMenu(fileName = "StaticOverworld_Behaviour", menuName = "Scriptable Objects/Behaviour/EntityOverworld/NPCOverworld/StaticOverworldBehaviourSO")]
    public class StaticOverworldBehaviourSO : NPCOverworldBehaviourSO
    {
        public override void BehaviourStart(EntityOverworldController controller)
        {
            if (controller.InstanceData is not StaticOverworldBehaviourInstanceData)
                controller.InstanceData = new StaticOverworldBehaviourInstanceData();

            controller.AIPath.orientation = Pathfinding.OrientationMode.YAxisForward;
            controller.AIPath.maxSpeed = Speed;
            controller.AIPath.maxAcceleration = Acceleration;
            controller.AIPath.pickNextWaypointDist = controller.Radius * 3;
            controller.AIPath.slowdownDistance = controller.Radius * 4;
            controller.AIPath.endReachedDistance = controller.Radius * 2;
        }

        public override void BehaviourUpdate(EntityOverworldController controller, float dt)
        {
            if (controller.InstanceData is not StaticOverworldBehaviourInstanceData instanceData)
                return;

            instanceData.facingDire = instanceData.defaultDirection;
        }

    }
}