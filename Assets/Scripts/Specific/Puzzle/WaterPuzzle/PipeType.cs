using System;


namespace Puzzle
{
    [System.Serializable]
    public class PipeType
    {
        public string pipeName;
        public PipeConnections baseConnections;

        public PipeConnections GetConnections(int rotationSteps)
        {
            var result = PipeConnections.None;
            foreach (PipeConnections side in (PipeConnections[])Enum.GetValues(typeof(PipeConnections)))
            {
                if ((baseConnections & side) != 0)
                    result |= RotateSide(side, rotationSteps);
            }
                
            return result;
        }

        private static PipeConnections RotateSide(PipeConnections side, int steps)
        {
            PipeConnections[] order = { PipeConnections.Up, PipeConnections.Right, PipeConnections.Down, PipeConnections.Left };
            int idx = System.Array.IndexOf(order, side);
            return order[(idx + steps) % 4];
        }
    }
}
