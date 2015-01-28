using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EpPathFinding.cs;

public class Rayman
{
    public bool[][] Walkable;
    public int offX;
    public int offZ;

    public StaticGrid EGrid;

    private JumpPointParam jpp = null;

    private static Rayman r = null;

    public static void Rebuild()
    {
        //Debug.Log("RB");
        r = new Rayman();
    }

    public static Rayman Get()
    {
        return r;
    }

    public Rayman()
    {
        List<IntBounds> obstacleBounds = new List<IntBounds>();

        foreach (Obstacle o in Grid.Get().Obstacles)
            if (o.Blocking)
                obstacleBounds.Add(o.IntBounds);

        //
        // find total grid bounds
        // 

        int mx = 0, mz = 0, Mx = 0, Mz = 0;

        foreach (IntBounds ib in obstacleBounds)
        {
            mx = Mathf.Min(mx, ib.minX);
            mz = Mathf.Min(mz, ib.minZ);
            Mx = Mathf.Max(Mx, ib.maxX);
            Mz = Mathf.Max(Mz, ib.maxZ);
        }

        //Debug.LogFormat("total FreeNa grid bounds: {0}x{1}  // {2}{3}", Mx - mx, Mz - mz, Mx - mx + 3, Mz - mz + 3);

        offX = mx - 1;
        offZ = mz - 1;

        Walkable = new bool[Mx - mx + 3][];

        for (int i = 0; i < Walkable.Length; i++)
        {
            Walkable[i] = new bool[Mz - mz + 3];
            for (int j = 0; j < Walkable[i].Length; j++)
                Walkable[i][j] = true;
        }

        //
        // mark up 2D bool grid
        //

        foreach (IntBounds ib in obstacleBounds)
        {
            for (int z = ib.minZ; z <= ib.maxZ; z++)
                for (int x = ib.minX; x <= ib.maxX; x++)
                {
                    Walkable[x - offX][z - offZ] = false;
                }
        }

        // feed to EpPathFinding.cs
        EGrid = new StaticGrid(Mx - mx + 3, Mz - mz + 3, Walkable);
    }

    public List<IntVector2> FindPath(IntVector2 start, IntVector2 end)
    {
        //Debug.Log("FP");

        bool startPosInBoundsX = start.x - offX >= 0 && start.x - offX < Walkable.Length;
        bool startPosInBoundsZ = start.z - offZ >= 0 && start.z - offZ < Walkable[0].Length;
        bool endPosInBoundsX = end.x - offX >= 0 && end.x - offX < Walkable.Length;
        bool endPosInBoundsZ = end.z - offZ >= 0 && end.z - offZ < Walkable[0].Length;

        var maxX = Walkable.Length - 1;
        var maxZ = Walkable[0].Length - 1;
        var minX = 0;
        var minZ = 0;

        var gspX = startPosInBoundsX ? start.x - offX : start.x - offX > 0 ? Walkable.Length - 1 : 0;
        var gspZ = startPosInBoundsZ ? start.z - offZ : start.z - offZ > 0 ? Walkable[0].Length - 1 : 0;
        var gepX = endPosInBoundsX ? end.x - offX : end.x - offX > 0 ? Walkable.Length - 1 : 0;
        var gepZ = endPosInBoundsZ ? end.z - offZ : end.z - offZ > 0 ? Walkable[0].Length - 1 : 0;

        if ((gspX == maxX && gepX == maxX) || (gspX == minX && gepX == minX) || (gspZ == maxZ && gepZ == maxZ) || (gspZ == minZ && gepZ == minZ))
        {
            // Early return. (TODO: should be able to handle all cases where both nodes are out of bounds, not only when on same side of collision grid :<)
            // (maybe do full pathfind, and if all nodes are on the border then roughly follow the path delta but then snap start & end?)
            return new IntVector2[] { end }.ToList();
        }

        GridPos startPos = new GridPos(gspX, gspZ);
        GridPos endPos = new GridPos(gepX, gepZ);
        //GridPos startPos = new GridPos(startPosInBoundsX ? start.x - offX : start.x - offX >= 0 ? )

        //GridPos startPos = new GridPos(start.x - offX, start.z - offZ);
        //GridPos endPos = new GridPos(end.x - offX, end.z - offZ);

        //Debug.LogFormat("{0} {1} {2} {3} {4} {5} // {6} {7}", offX, offZ, gspX, gspZ, gepX, gepZ, Walkable.Length - 1, Walkable[0].Length - 1);
        if (jpp == null)
            jpp = new JumpPointParam(EGrid, startPos, endPos, true, true, true, HeuristicMode.EUCLIDEAN);
        else
            jpp.Reset(startPos, endPos);
        
        List<GridPos> results = JumpPointFinder.FindPath(jpp);

        //if (!startPosInBoundsX || !startPosInBoundsZ)
        //    results.Insert(0, new GridPos(start.x - offX, start.z - offZ));
        if ((!endPosInBoundsX || !endPosInBoundsZ) && endPos == results[results.Count - 1])
            results.Add(new GridPos(end.x - offX, end.z - offZ));

        // Postprocess: Returned first point must never be the same as what was passed in.
        if (results.Count > 0 && results[0].x + offX == start.x && results[0].y + offZ == start.z)
            results.RemoveAt(0);

        // Postprocess: If list is empty, just put in the end point. This overrides the 'don't return start pos' rule.
        if (results.Count == 0)
            results.Add(new GridPos(end.x - offX, end.z - offZ));

        return results.ConvertAll<IntVector2>((gp) => new IntVector2(gp.x + offX, gp.y + offZ));
    }

    public bool IsBlocked(IntVector2 gv) { return IsBlocked(gv.x, gv.z); }
    public bool IsBlocked(int gx, int gz)
    {
        bool posInBoundsX = gx - offX >= 0 && gx - offX < Walkable.Length;
        bool posInBoundsZ = gz - offZ >= 0 && gz - offZ < Walkable[0].Length;
        return (!posInBoundsX || !posInBoundsZ) ? true : !Walkable[gx - offX][gz - offZ];
    }
}

