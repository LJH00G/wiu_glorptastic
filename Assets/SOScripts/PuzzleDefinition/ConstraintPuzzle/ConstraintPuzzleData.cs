using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    [CreateAssetMenu(fileName = "ConstraintPuzzleData", menuName = "PuzzleData/Constraint")]
    public class ConstraintPuzzleData : PuzzleData
    {
        public SeatLayout layout;
        public string[] people;

        [SerializeReference] public List<SeatConstraint> constraints = new();

        public override bool CheckSolution(object attempt)
        {
            var arrangement = attempt as Dictionary<string, string>;
            if (arrangement == null) return false;

            if (arrangement.Count != people.Length) return false; 

            foreach (var constraint in constraints)
            {
                if (!constraint.IsSatisfied(arrangement, layout))
                    return false;
            }
            return true;
        }

#if UNITY_EDITOR
        [ContextMenu("Auto-Generate Seat IDs")]
        private void AutoGenerateSeatIds()
        {
            if (layout.rows <= 0 || layout.cols <= 0)
            {
                Debug.LogError("Set rows/cols before generating seat IDs.");
                return;
            }

            layout.seatID = new string[layout.rows * layout.cols];
            for (int r = 0; r < layout.rows; r++)
            {
                for (int c = 0; c < layout.cols; c++)
                    layout.seatID[r * layout.cols + c] = $"R{r}C{c}";
            }
                

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}

