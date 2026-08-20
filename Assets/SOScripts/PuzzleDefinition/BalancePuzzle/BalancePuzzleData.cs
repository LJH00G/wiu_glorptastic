using UnityEngine;
using System.Collections.Generic;

namespace Puzzle
{

    [CreateAssetMenu(fileName = "BalancePuzzleData", menuName = "PuzzleData/Balance")]
    public class BalancePuzzleData : PuzzleData
    {
        public List<BallSlot> slots = new();
        public List<BallWeight> balls = new();

        public Vector2 targetOffset;
        public float tolerance = 0.1f;

        public override bool CheckSolution(object attempt)
        {
            var placement = attempt as Dictionary<string, string>;
            if (placement == null) return false;

            Vector2 centerOfMass = ComputeCenterOfMass(placement);
            return Vector2.Distance(centerOfMass, targetOffset) <= tolerance;
        }

        public Vector2 ComputeCenterOfMass(Dictionary<string, string> placement)
        {
            Vector2 weightedSum = Vector2.zero;
            float totalMass = 0f;

            foreach (var kv in placement)
            {
                var slot = slots.Find(s => s.slotId == kv.Key);
                var ball = balls.Find(b => b.ballId == kv.Value);
                if (slot == null || ball == null) continue;

                weightedSum += slot.localPosition * ball.mass;
                totalMass += ball.mass;
            }

            return totalMass > 0f ? weightedSum / totalMass : Vector2.zero;
        }
    }
}