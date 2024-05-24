using System;

namespace Gameplay
{
    [Flags]
    public enum Side
    {
        None = 0,
        Right = 1,
        Left = 2,
        Centered = Right | Left
    }
}
