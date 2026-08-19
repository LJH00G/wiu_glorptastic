
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
        public override void BehaviourStart(EntityOverworldController controller)
        {
            if (controller.InstanceData is not FollowObjectOverworldBehaviourInstanceData)
                controller.InstanceData = new FollowObjectOverworldBehaviourInstanceData();
        }

        public override void BehaviourUpdate(EntityOverworldController controller, float dt)
        {
            throw new System.NotImplementedException();
        }

    }
}