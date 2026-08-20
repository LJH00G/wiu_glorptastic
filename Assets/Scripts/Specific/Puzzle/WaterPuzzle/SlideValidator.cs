using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Puzzle
{
    public enum SlideDirection 
    { 
        Up, 
        Down, 
        Left, 
        Right 
    }

    public static class SlideValidator
    {
        public static Vector2Int SlideToStop(PipePieceInstance piece, SlideDirection direction, List<PipePieceInstance> allPieces, int rows, int cols)
        {
            Vector2Int delta = DirectionToDelta(direction);
            Vector2Int current = piece.position;

            while (true)
            {
                var next = current + delta;

                if (next.x < 0 || next.x >= cols || next.y < 0 || next.y >= rows) 
                    break;

                if (allPieces.Any(p => p != piece && p.position == next)) 
                    break;

                current = next;
            }
            return current;
        }

        private static Vector2Int DirectionToDelta(SlideDirection direction) => direction switch
        {
            SlideDirection.Up => Vector2Int.up,
            SlideDirection.Down => Vector2Int.down,
            SlideDirection.Left => Vector2Int.left,
            SlideDirection.Right => Vector2Int.right,
            _ => Vector2Int.zero
        };
    }
}
