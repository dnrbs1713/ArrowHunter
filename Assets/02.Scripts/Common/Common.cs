using UnityEngine;

namespace ArrowClash.Common
{
    public enum Direction
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }

    public enum DurationRule
    {
        Ignore,
        Refresh,
        Stack,
    }

    public enum StackRule
    {
        Ignore,
        Stack,
    }
}
