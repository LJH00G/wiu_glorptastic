using System;

public interface IPuzzleView
{
    void Load(PuzzleData data);
    event Action<bool> OnSolutionSubmitted;
}
