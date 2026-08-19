
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using UnityEngine;


namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    public class EnemyOverworldBehaviourInstanceData : EntityOverworldBehaviourInstanceData
    {

    }
}


namespace Game.SO.Behaviour.EntityOverworld
{
    [CreateAssetMenu(fileName = "EnemyOverworld_Behaviour", menuName = "Scriptable Objects/Behaviour/EntityOverworld/NPCOverworld/EnemyOverworldBehaviourSO")]
    public class EnemyOverworldBehaviourSO : NPCOverworldBehaviourSO
    {
        public override void BehaviourStart(EntityOverworldController controller)
        {
            if (controller.InstanceData is not EnemyOverworldBehaviourInstanceData)
                controller.InstanceData = new EnemyOverworldBehaviourInstanceData();
        }

        public override void BehaviourUpdate(EntityOverworldController controller, float dt)
        {
            throw new System.NotImplementedException();
        }

    }
}