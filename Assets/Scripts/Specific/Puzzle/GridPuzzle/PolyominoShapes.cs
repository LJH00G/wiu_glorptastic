using UnityEngine;

[System.Serializable]
public class PolyominoShape
{
    public string shapeName; 
    public Vector2Int[] cells;

    public Vector2Int[] Rotated(int quarterTurns)
    {
        var result = new Vector2Int[cells.Length];
        for (int i = 0; i < cells.Length; i++)
        {
            var currentCell = cells[i];
            for (int t = 0; t < quarterTurns; t++)
            {
                currentCell = new Vector2Int(-currentCell.y, currentCell.x);
            }
                
            result[i] = currentCell;
        }
        return result;
    }
}