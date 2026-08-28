using UnityEngine;
using Utility.VisualizableDictionary;

namespace Game.GlobalVariable.OverworldNPCMovePoints
{
    [DefaultExecutionOrder(-9999)]
    public class OverworldNPCMovePointsManager : MonoBehaviour
    {
        [field: SerializeField]
        public VisualizableDict<string, MovePoints> MovePointsList { get; private set; } = new();

        void ReconstructGlobalVariable()
        {
            MovePointsList.OnValidate();
            OverworldNPCMovePointsGlobalVariable.MovePointsDict.Clear();
            OverworldNPCMovePointsGlobalVariable.MovePointsDict = new(MovePointsList.dict);
        }

        private void Awake()
        {
            ReconstructGlobalVariable();
        }


#if UNITY_EDITOR

        private void OnValidate()
        {
            MovePointsList.OnValidate();
            ReconstructGlobalVariable();
        }

        private void OnDrawGizmosSelected()
        {
            foreach (var entry in MovePointsList.dict)
            {
                var points = entry.Value.points;
                if (points.Length == 0)
                    continue;

                Gizmos.color = entry.Value.debugColor;
                Vector2 prevPoint = points[^1];
                for (int i = 0; i < points.Length; i++)
                {
                    Vector2 point = points[i];

                    Gizmos.DrawLine(prevPoint, point);
                    Gizmos.DrawSphere(point, 0.1f);

                    prevPoint = point;
                }
            }
        }

#endif

    }
}