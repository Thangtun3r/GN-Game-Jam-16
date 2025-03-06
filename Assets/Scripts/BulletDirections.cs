using UnityEngine;

[System.Flags]
public enum BulletDirections
{
    None              = 0,
    Up                = 1 << 0,
    Down              = 1 << 1,
    Left              = 1 << 2,
    Right             = 1 << 3,
    DiagonalUpLeft    = 1 << 4,
    DiagonalUpRight   = 1 << 5,
    DiagonalDownLeft  = 1 << 6,
    DiagonalDownRight = 1 << 7
}