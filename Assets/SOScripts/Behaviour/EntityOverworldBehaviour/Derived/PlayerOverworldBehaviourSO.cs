
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;


namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    public class PlayerOverworldBehaviourInstanceData : EntityOverworldBehaviourInstanceData
    {

    }
}


namespace Game.SO.Behaviour.EntityOverworld
{
    [CreateAssetMenu(fileName = "PlayerOverworld_Behaviour", menuName = "Scriptable Objects/Behaviour/EntityOverworld/PlayerOverworldBehaviourSO")]
    public class PlayerOverworldBehaviourSO : EntityOverworldBehaviourSO
    {
        public override void BehaviourStart(EntityOverworldController controller)
        {
            if (controller.InstanceData is not PlayerOverworldBehaviourInstanceData)
                controller.InstanceData = new PlayerOverworldBehaviourInstanceData();

            controller.AIPath.orientation = Pathfinding.OrientationMode.YAxisForward;
            controller.AIPath.maxSpeed = speed;
            controller.AIPath.maxAcceleration = acceleration;
            controller.AIPath.pickNextWaypointDist = 0.5f;
            controller.AIPath.slowdownDistance = 0;
            controller.AIPath.endReachedDistance = 0;

        }

        public override void BehaviourUpdate(EntityOverworldController controller, float dt)
        {
            Vector2 moveDire = InputSystem.actions["Move"].ReadValue<Vector2>();

            controller.AIPath.destination = (Vector2)controller.transform.position + moveDire * 0.1f;
        }

    }
}