using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShapePlacement
{
    public string shapeName;
    public Vector2Int anchor;
    public int rotation; 

    public IEnumerable<Vector2Int> GetOccupiedCells(PolyominoShape shape)
    {
        foreach (var offset in shape.Rotated(rotation))
            yield return anchor + offset;
    }
}