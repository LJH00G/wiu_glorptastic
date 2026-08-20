
namespace Puzzle
{
    [System.Flags]
    public enum PipeConnections
    {
        None = 0,
        Up = 1,
        Right = 1 << 1,
        Down = 1 << 2,
        Left =  1 << 3
    }
}