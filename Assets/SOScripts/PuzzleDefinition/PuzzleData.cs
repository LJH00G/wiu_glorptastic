using UnityEngine;
using Puzzle;

[CreateAssetMenu(fileName = "PuzzleData", menuName = "PuzzleData/BasePuzzleData")]
public abstract class PuzzleData : ScriptableObject
{
    public PuzzleType puzzleType;
    public string puzzleName;

    public abstract bool CheckSolution(object attempt);
}
