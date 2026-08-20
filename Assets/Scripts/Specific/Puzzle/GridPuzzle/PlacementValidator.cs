using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    public static class PlacementValidator
    {
        
        public static bool IsValidPlacement(ShapePlacement placement, PolyominoShape shape, int rows, int cols, bool[,] existingFilled)
        {
            foreach (var cell in placement.GetOccupiedCells(shape))
            {
                if (cell.x < 0 || cell.x >= cols || cell.y < 0 || cell.y >= rows)
                {
                    return false;
                }
                    
                if (existingFilled[cell.y, cell.x])
                {
                    return false;
                }
                    
            }
            return true;
        }

        
        public static bool[,] BuildFilledGrid( List<ShapePlacement> placements, List<PolyominoShape> availableShapes, int rows, int cols)
        {
            bool[,] filled = new bool[rows, cols];

            foreach (var placement in placements)
            {
                var shape = availableShapes.Find(s => s.shapeName == placement.shapeName);

                if (shape == null)
                {
                    return null;
                }
                

                if (!IsValidPlacement(placement, shape, rows, cols, filled))
                {
                    return null;
                }
                    

                foreach (var cell in placement.GetOccupiedCells(shape))
                {
                    filled[cell.y, cell.x] = true;
                }
                    
            }
            return filled;
        }
    }
}
