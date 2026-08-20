
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore;


namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    public class PlayerOverworldBehaviourInstanceData : EntityOverworldBehaviourInstanceData
    {
        public Vector2 facingDire;
    }
}


namespace Game.SO.Behaviour.EntityOverworld
{
    [CreateAssetMenu(fileName = "PlayerOverworld_Behaviour", menuName = "Scriptable Objects/Behaviour/EntityOverworld/PlayerOverworldBehaviourSO")]
    public class PlayerOverworldBehaviourSO : EntityOverworldBehaviourSO
    {
        [Header("Player")]
        [SerializeField] float interactionSize;

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
            if (controller.InstanceData is not PlayerOverworldBehaviourInstanceData instanceData)
                return;
            

            // movement
            Vector2 dire = InputSystem.actions["Move"].ReadValue<Vector2>();

            if (dire != Vector2.zero)
                instanceData.facingDire = dire;

            controller.AIPath.destination = (Vector2)controller.transform.position + dire * 0.1f;

            
            // interact
            if (InputSystem.actions["Interact"].triggered) // && GameManager.CanInteract
            {
                Vector2 offset = instanceData.facingDire * (controller.Radius + interactionSize * 0.5f);

                var colliders = Physics2D.OverlapBoxAll(
                        (Vector2)controller.transform.position + offset,
                        new Vector2(interactionSize, interactionSize),
                        0,
                        LayerMask.NameToLayer("Interactable")
                    );

                if (colliders.Length > 0)
                {
                    var collider = colliders[0];

                }

            }
        }

    }
}