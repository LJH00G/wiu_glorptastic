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

        

        public static Vector2Int GetOneStepMove(PipePieceInstance piece, SlideDirection direction, List<PipePieceInstance> allPieces, int rows, int cols)
        {
            Vector2Int delta = DirectionToDelta(direction);
            Vector2Int target = piece.position + delta;

            if (target.x < 0 || target.x >= cols || target.y < 0 || target.y >= rows)
                return piece.position;

            if (allPieces.Any(p => p != piece && p.position == target))
                return piece.position;

            return target;
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