using UnityEngine;
using System.Collections.Generic;

namespace Puzzle
{
    [System.Serializable]
    public abstract class SeatConstraint
    {
        public abstract bool IsSatisfied(Dictionary<string, string> constraintData, SeatLayout layout);
        public abstract string Describe();

        protected string FindSeat(Dictionary<string,string> arrangement, string personID)
        {
            foreach (var kv in arrangement)
            {
                if (kv.Value == personID) 
                    return kv.Key;
            }
                
            return null;
        }
    }
}
