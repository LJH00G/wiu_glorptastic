using System;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    [CreateAssetMenu(fileName = "WaterPuzzleData", menuName = "PuzzleData/Water")]
    public class WaterPuzzleData : PuzzleData
    {
        public int rows;
        public int cols;
        public List<PipeType> availablePipes = new();
        public List<PipePieceInstance> startingPieces = new(); 

        public Vector2Int sourceCell;
        public PipeConnections sourceExitSide;
        public Vector2Int targetCell;
        public PipeConnections targetEntrySide;

        public override bool CheckSolution(object attempt)
        {
            var pieces = attempt as List<PipePieceInstance>;
            if (pieces == null) return false;

            var grid = new PipeCell?[rows, cols];
            foreach (var piece in pieces)
                grid[piece.position.y, piece.position.x] = new PipeCell { pipeName = piece.pipeName, rotation = piece.rotation };

            return TraceFlow(grid);
        }

        private bool TraceFlow(PipeCell?[,] grid)
        {
            var visited = new HashSet<Vector2Int>();
            var current = sourceCell;
            var enteringFrom = Opposite(sourceExitSide);

            while (true)
            {
                if (current.x < 0 || current.x >= cols || current.y < 0 || current.y >= rows) 
                    return false;

                if (!visited.Add(current)) 
                    return false;

                var cellData = grid[current.y, current.x];

                if (cellData == null) 
                    return false;

                var pipeType = availablePipes.Find(p => p.pipeName == cellData.Value.pipeName);

                if (pipeType == null) 
                    return false;

                var connections = pipeType.GetConnections(cellData.Value.rotation);

                if (!HasFlag(connections, enteringFrom)) 
                    return false;

                var exitSide = FindExit(connections, enteringFrom);

                if (exitSide == PipeConnections.None) 
                    return false;

                if (current == targetCell)
                {
                    return exitSide == targetEntrySide || HasFlag(connections, targetEntrySide);

                }
                

                current = Step(current, exitSide);
                enteringFrom = Opposite(exitSide);
            }
        }

        private static bool HasFlag(PipeConnections val, PipeConnections flag)
        {
            return (val & flag) != 0;
        }
        

        private static PipeConnections Opposite(PipeConnections side) => side switch
        {
            PipeConnections.Up => PipeConnections.Down,
            PipeConnections.Down => PipeConnections.Up,
            PipeConnections.Left => PipeConnections.Right,
            PipeConnections.Right => PipeConnections.Left,
            _ => PipeConnections.None
        };

        private static PipeConnections FindExit(PipeConnections connections, PipeConnections excludeSide)
        {
            foreach (PipeConnections side in (PipeConnections[])Enum.GetValues(typeof(PipeConnections)))

                if (side != excludeSide && HasFlag(connections, side))
                    return side;

            return PipeConnections.None;
        }

        private static Vector2Int Step(Vector2Int pos, PipeConnections direction) => direction switch
        {
            PipeConnections.Up => pos + Vector2Int.up,
            PipeConnections.Down => pos + Vector2Int.down,
            PipeConnections.Left => pos + Vector2Int.left,
            PipeConnections.Right => pos + Vector2Int.right,
            _ => pos
        };
    }
}