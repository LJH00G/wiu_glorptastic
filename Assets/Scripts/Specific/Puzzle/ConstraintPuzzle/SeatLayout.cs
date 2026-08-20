using System;
using UnityEngine;

namespace Puzzle
{
    [System.Serializable]
    public class SeatLayout
    {
        public int rows;
        public int cols;
        public string[] seatID;

        public bool AreAdjacent(string seatA, string seatB)
        {
            Vector2Int A = GetCoords(seatA);
            Vector2Int B = GetCoords(seatB);
            int dr = Math.Abs(A.x - B.x), dc = Math.Abs(A.y - B.y);

            return (dr == 1 && dc == 0) || (dr == 0 && dc == 1);
        }

        public int GetRow(string seatId)
        {
            return GetCoords(seatId).x;
        }
        
        public int GetCol(string seatId)
        {
            return GetCoords(seatId).y;
        }

        private Vector2Int GetCoords(string seatId)
        {
            int idx = Array.IndexOf(seatID, seatId);
            if (idx < 0)
            {
                return new Vector2Int(-1, -1);
            }

            return new Vector2Int(idx / cols, idx % cols);
        }


    }
}

