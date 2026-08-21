
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.TriggerHandler;


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
            Vector2 dire = GameManager.PlayerCanMove ?
                InputSystem.actions["Move"].ReadValue<Vector2>() : Vector2.zero;

            if (dire != Vector2.zero)
                instanceData.facingDire = dire;

            controller.AIPath.destination = (Vector2)controller.transform.position + dire * 0.1f;


            // interact
            if (InputSystem.actions["Interact"].WasPressedThisFrame() && GameManager.CanInteract)
            {
                Vector2 offset = instanceData.facingDire * (controller.Radius + interactionSize * 0.5f);
                Vector2 interactionPos = (Vector2)controller.transform.position + offset;
                Vector2 interactionSize_vec2 = new Vector2(interactionSize, interactionSize);

                var colliders = Physics2D.OverlapBoxAll(
                        interactionPos,
                        interactionSize_vec2,
                        0,
                        LayerMask.GetMask("Interactable")
                    );

                DebugDraw.Box(interactionPos, interactionSize_vec2);


                foreach (var collider in colliders)
                {
                    if (collider.TryGetComponent(out I_TriggerHandler handler) &&
                        handler.RequiresInteraction())
                    {
                        handler?.Trigger();
                        break;
                    }
                }
                    

            }
        }

    }
}