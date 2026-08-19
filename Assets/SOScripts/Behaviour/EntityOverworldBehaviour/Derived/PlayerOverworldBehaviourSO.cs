
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    public class PlayerOverworldBehaviourInstanceData : EntityOverworldBehaviourInstanceData
    {
        public Transform targetForm;
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
        }

        public override void BehaviourUpdate(EntityOverworldController controller, float dt)
        {
            Vector2 moveDire = InputSystem.actions["Move"].ReadValue<Vector2>();

            // do stuff 

        }

    }
}