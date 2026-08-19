
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using UnityEngine;


namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    public class MovePointsOverworldBehaviourInstanceData : EntityOverworldBehaviourInstanceData
    {
        public int toPointIndex;
        public int fromPointIndex;
        public bool isIncrement;
    }
}


namespace Game.SO.Behaviour.EntityOverworld
{
    [CreateAssetMenu(fileName = "MovePointsOverworld_Behaviour", menuName = "Scriptable Objects/Behaviour/EntityOverworld/NPCOverworld/MovePointsOverworldBehaviourSO")]
    public class MovePointsOverworldBehaviourSO : NPCOverworldBehaviourSO
    {
        [SerializeField] Vector2[] points;
        [SerializeField] bool isCycle;
        [SerializeField] float maxPauseDurationAtPoint;


        public override void BehaviourStart(EntityOverworldController controller)
        {
            if (controller.InstanceData is not MovePointsOverworldBehaviourInstanceData)
                controller.InstanceData = new MovePointsOverworldBehaviourInstanceData();


            MovePointsOverworldBehaviourInstanceData instanceData = (MovePointsOverworldBehaviourInstanceData)controller.InstanceData;

            instanceData.isIncrement = Random.Range(0, 1) == 1;
        }

        public override void BehaviourUpdate(EntityOverworldController controller, float dt)
        {
            throw new System.NotImplementedException();
        }


#if UNITY_EDITOR
        public override void BehaviourOnDrawGizmosSelected(EntityOverworldController controller)
        {
            if (points.Length == 0)
                return;

            if (controller.InstanceData is not MovePointsOverworldBehaviourInstanceData instanceData)
                return;

            Gizmos.color = Color.blue;

            Vector2 prevPoint = points[^1];
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 point = points[i];

                Gizmos.DrawLine(prevPoint, point);
                if (i == instanceData.toPointIndex)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(point, 0.2f);
                    Gizmos.color = Color.blue;
                }
                else
                    Gizmos.DrawSphere(point, 0.1f);

                prevPoint = point;
            }
            
        }
#endif
    }
}