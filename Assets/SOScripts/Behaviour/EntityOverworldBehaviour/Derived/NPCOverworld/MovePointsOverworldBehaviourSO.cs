
using Game.SO.Behaviour.EntityOverworld.InstanceData;
using UnityEngine;
using Utility.Math;
using Game.GlobalVariable.OverworldNPCMovePoints;


namespace Game.SO.Behaviour.EntityOverworld.InstanceData
{
    [System.Serializable]
    public class MovePointsOverworldBehaviourInstanceData : EntityOverworldBehaviourInstanceData
    {
        public string MovePointsKey;
        public int toPointIndex;
        public bool isIncrement;
        public float pauseTimer;
    }
}


namespace Game.SO.Behaviour.EntityOverworld
{
    [CreateAssetMenu(fileName = "MovePointsOverworld_Behaviour", menuName = "Scriptable Objects/Behaviour/EntityOverworld/NPCOverworld/MovePointsOverworldBehaviourSO")]
    public class MovePointsOverworldBehaviourSO : NPCOverworldBehaviourSO
    {
        [Header("Move Points")]
        [SerializeField] bool isCycle;
        [SerializeField] bool isRandom;
        [SerializeField] float maxPauseDurationAtPoint;


        public override void BehaviourStart(EntityOverworldController controller)
        {
            if (controller.InstanceData is not MovePointsOverworldBehaviourInstanceData)
                controller.InstanceData = new MovePointsOverworldBehaviourInstanceData();

            controller.AIPath.orientation = Pathfinding.OrientationMode.YAxisForward;
            controller.AIPath.maxSpeed = Speed;
            controller.AIPath.maxAcceleration = Acceleration;
            controller.AIPath.pickNextWaypointDist = controller.Radius * 3;
            controller.AIPath.slowdownDistance = controller.Radius * 4;
            controller.AIPath.endReachedDistance = controller.Radius;

            MovePointsOverworldBehaviourInstanceData instanceData = (MovePointsOverworldBehaviourInstanceData)controller.InstanceData;

            if (isCycle)
                instanceData.isIncrement = Random.Range(0, 1) == 1;

            int closestPointIndex = 0;
            float closestDist_sqr = float.PositiveInfinity;

            if (!OverworldNPCMovePointsGlobalVariable.MovePointsDict.TryGetValue(instanceData.MovePointsKey, out MovePoints movePoints))
                return;
            Vector2[] points = movePoints.points;

            for (int i = 0; i < points.Length; i++)
            {
                float dist_sqr = (points[i] - (Vector2)controller.transform.position).sqrMagnitude;

                if (i == 0)
                {
                    closestDist_sqr = dist_sqr;
                    continue;
                }

                if (dist_sqr < closestDist_sqr)
                {
                    closestDist_sqr = dist_sqr;
                    closestPointIndex = i;
                }
            }

            instanceData.toPointIndex = closestPointIndex;
        }

        public override void BehaviourUpdate(EntityOverworldController controller, float dt)
        {
            if (controller.InstanceData is not MovePointsOverworldBehaviourInstanceData instanceData)
                return;

            if (!OverworldNPCMovePointsGlobalVariable.MovePointsDict.TryGetValue(instanceData.MovePointsKey, out MovePoints movePoints))
                return;
            Vector2[] points = movePoints.points;

            controller.AIPath.destination = points[instanceData.toPointIndex];

            if (controller.AIPath.reachedEndOfPath)
            {
                if (instanceData.pauseTimer <= 0)
                    instanceData.pauseTimer = Random.Range(maxPauseDurationAtPoint * 0.5f, maxPauseDurationAtPoint);

                instanceData.pauseTimer -= dt;

                if (instanceData.pauseTimer <= 0)
                {
                    if (isRandom)
                        instanceData.toPointIndex = Random.Range(0, points.Length - 1);
                    else
                        instanceData.toPointIndex = instanceData.isIncrement ?
                            Math_I.IncrementWrap(instanceData.toPointIndex, 0, points.Length - 1) :
                            Math_I.DecrementWrap(instanceData.toPointIndex, 0, points.Length - 1);
                }
            }
        }


#if UNITY_EDITOR

        public override void BehaviourOnValidate(EntityOverworldController controller)
        {
            if (controller.InstanceData is not MovePointsOverworldBehaviourInstanceData instanceData)
                return;

            if (!OverworldNPCMovePointsGlobalVariable.MovePointsDict.TryGetValue(instanceData.MovePointsKey, out MovePoints movePoints))
            {
                Debug.LogError($"no MovePoints in this scene matches the key {instanceData.MovePointsKey}", this);
                return;
            }

            if (instanceData.toPointIndex < 0)
                instanceData.toPointIndex = movePoints.points.Length - 1;
            else if (instanceData.toPointIndex >= movePoints.points.Length)
                instanceData.toPointIndex = 0;
            
        }

        public override void BehaviourOnDrawGizmosSelected(EntityOverworldController controller)
        {
            if (controller.InstanceData is not MovePointsOverworldBehaviourInstanceData instanceData)
                return;

            if (!OverworldNPCMovePointsGlobalVariable.MovePointsDict.TryGetValue(instanceData.MovePointsKey, out MovePoints movePoints))
                return;
            Vector2[] points = movePoints.points;

            if (points.Length == 0)
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