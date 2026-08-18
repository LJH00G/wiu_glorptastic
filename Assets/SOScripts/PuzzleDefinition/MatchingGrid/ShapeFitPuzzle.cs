using UnityEngine;
using System.Collections.Generic;
using System;

namespace Puzzle
{
    [CreateAssetMenu(fileName = "ShapeFitPuzzleData", menuName = "PuzzleData/ShapeFit")]

    
    public class ShapeFitPuzzleData : PuzzleData
    {
        public int rows;
        public int cols;

        public List<PolyominoShape> availableShapes = new();
        public Clue[] rowRuns; 
        public Clue[] colRuns;
       
        public bool RequireAllShapesPlaced = true;

        public override bool CheckSolution(object attempt)
        {
            var placements = attempt as List<ShapePlacement>;
            if (placements == null) return false;

            if (RequireAllShapesPlaced && placements.Count != availableShapes.Count)
            {
                return false;
            }
                

            var filled = PlacementValidator.BuildFilledGrid(placements, availableShapes, rows, cols);

            if (filled == null)
            {
                return false;
            }

            for (int r = 0; r < rows; r++)
            {
                var rowFilled = new List<bool>();

                for (int c = 0; c < cols; c++)
                {
                    rowFilled.Add(filled[r, c]);
                }
                
                if (!RunClueChecker.Matches(rowFilled, rowRuns[r].run))
                {
                    return false;
                }
                
            }

            for (int c = 0; c < cols; c++)
            {
                var colFilled = new List<bool>();
                for (int r = 0; r < rows; r++)
                {
                    colFilled.Add(filled[r, c]);
                }
                
                if (!RunClueChecker.Matches(colFilled, colRuns[c].run))
                {
                    return false;
                }
            }

            return true;
        }
    }
}