using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct IntVector2
{
    public int x;
    public int z;

    public IntVector2(int x, int z) { this.x = x; this.z = z; }
    public override string ToString() { return "(" + x + "," + z + ")"; }
}

public static class IntVector2Extensions
{
    public static IntVector2 ToIntVector2(this Vector3 v)
    {
        return new IntVector2(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.z));
    }
}
