using System;
using ArrowClash.Common;
using UnityEngine.Experimental.GlobalIllumination;

[Serializable]
public struct DirectionalInt
{
    public int up;
    public int down;
    public int left;
    public int right;
    
    public DirectionalInt(int up, int down, int left, int right)
    {
        this.up = up;   
        this.down = down;
        this.left = left;
        this.right = right;
    }
    public int Get(Direction dir)
    {
        return dir switch
        {
            Direction.Up => up,
            Direction.Down => down,
            Direction.Left => left,
            Direction.Right => right,
            _ => 0
        };
    }

    public void Set(Direction dir, int value)
    {
        switch (dir)
        {
            case Direction.Up: up = value; break;
            case Direction.Down: down = value; break;
            case Direction.Left: left = value; break;
            case Direction.Right: right = value; break;
        }
    }
}
