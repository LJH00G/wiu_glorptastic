using UnityEngine;
using System.Collections.Generic;

namespace Puzzle
{
    [System.Serializable]
    public class NotAdjacentConstraint : SeatConstraint
    {
        public string personA;
        public string personB;

        public override bool IsSatisfied(Dictionary<string, string> constraintData, SeatLayout layout)
        {
            string seatA = FindSeat(constraintData, personA);
            string seatB = FindSeat(constraintData, personB);
            if (seatA == null || seatB == null) return true;

            return !layout.AreAdjacent(seatA, seatB);
        }

        public override string Describe()
        {
            return $"{personA} does not sit next to {personB}.";
        }
    }
}
