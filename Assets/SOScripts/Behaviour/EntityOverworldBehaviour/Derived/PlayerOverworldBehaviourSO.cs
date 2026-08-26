
using Game.Interactable;
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    [Serializable]
    public class PlayerOverworldBehaviourInstanceData : EntityOverworldBehaviourInstanceData
    {
        
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
            controller.AIPath.maxSpeed = Speed;
            controller.AIPath.maxAcceleration = Acceleration;
            controller.AIPath.pickNextWaypointDist = controller.Radius;
            controller.AIPath.slowdownDistance = controller.Radius * 0.9f;
            controller.AIPath.endReachedDistance = controller.Radius * 0.75f;
            controller.AIPath.destination = controller.transform.position;

        }

        public override void BehaviourUpdate(EntityOverworldController controller, float dt)
        {
            if (controller.InstanceData is not PlayerOverworldBehaviourInstanceData instanceData)
                return;

            // movement
            Vector2 dire = GameManager.PlayerCanMove ?
                InputSystem.actions["Move"].ReadValue<Vector2>() : Vector2.zero;

            if (dire != Vector2.zero)
            {
                instanceData.SetFacingDire(dire);

                controller.AIPath.destination = (Vector2)controller.transform.position + dire * controller.Radius;
            }

            // interact
            if (InputSystem.actions["Interact"].WasPressedThisFrame() && GameManager.CanInteract)
            {
                Vector2 offset = instanceData.GetVector2Dire() * (controller.Radius + interactionSize * 0.5f);
                Vector2 interactionPos = (Vector2)controller.transform.position + offset;
                Vector2 interactionSize_vec2 = new(interactionSize, interactionSize);

                var colliders = Physics2D.OverlapBoxAll(
                        interactionPos,
                        interactionSize_vec2,
                        0,
                        LayerMask.GetMask("Interactable")
                    );

                DebugDraw.Box(interactionPos, interactionSize_vec2);


                foreach (var collider in colliders)
                {
                    if (collider.TryGetComponent(out I_Interactable interactable))
                    {
                        interactable?.Interact();
                        break;
                    }
                }


            }
        }

    }
}