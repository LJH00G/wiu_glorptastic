using System.Collections.Generic;

public static class RunClueChecker
{
    public static bool Matches(List<bool> filledCells, int[] expectedRuns)
    {
        var actualRuns = new List<int>();
        int current = 0;
        foreach (bool filled in filledCells)
        {
            if (filled) current++;
            else if (current > 0) { actualRuns.Add(current); current = 0; }
        }
        if (current > 0) actualRuns.Add(current);

        if (actualRuns.Count != expectedRuns.Length) 
            return false;

        for (int i = 0; i < actualRuns.Count; i++)
        {
            if (actualRuns[i] != expectedRuns[i])
                return false;
        }

            
        return true;
    }
}