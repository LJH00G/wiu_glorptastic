using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    public class SameRowConstraint : SeatConstraint
    {
        public string personA;
        public string personB;

        public override bool IsSatisfied(Dictionary<string, string> constraintData, SeatLayout layout)
        {
            string seatA = FindSeat(constraintData, personA);
            string seatB = FindSeat(constraintData, personB);
            if (seatA == null || seatB == null) return true;

            return layout.GetRow(seatA) == layout.GetRow(seatB);
        }

        public override string Describe()
        {
            return $"{personA} sits in the same row as {personB}.";
        }
    }
}

