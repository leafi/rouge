using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Globox
{
    public IntBounds Bounds;
    public List<int> Neighbours = new List<int>();
}

public class Rayman
{
    public List<Globox> BoxNa = new List<Globox>();
    public bool[,] Blocked;
    public int[,] CellOwnership;
    public int[,] RightMinesweeper;
    public int[,] DownMinesweeper;

    public int offX;
    public int offZ;

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

        Debug.LogFormat("total FreeNa grid bounds: {0}x{1}", Mx - mx, Mz - mz);

        offX = mx;
        offZ = mz;

        Blocked = new bool[Mx - mx + 1, Mz - mz + 1];
        CellOwnership = new int[Mx - mx + 1, Mz - mz + 1];
        RightMinesweeper = new int[Mx - mx + 1, Mz - mz + 1];
        DownMinesweeper = new int[Mx - mx + 1, Mz - mz + 1];

        //
        // mark up 2D bool grid
        //

        foreach (IntBounds ib in obstacleBounds)
        {
            for (int z = ib.minZ; z <= ib.maxZ; z++)
                for (int x = ib.minX; x <= ib.maxX; x++)
                {
                    Blocked[x - offX, z - offZ] = true;
                    CellOwnership[x - offX, z - offZ] = -1;
                }
        }

        var lenZ = Mz - mz + 1;
        var lenX = Mx - mx + 1;

        //
        // play right minesweeper
        //
        for (int z = 0; z < lenZ; z++)
        {
            RightMinesweeper[0, z] = Blocked[0, z] ? 0 : 1;
            for (int x = 1; x < lenX; x++)
                RightMinesweeper[x, z] = Blocked[x, z] ? 0 : RightMinesweeper[x - 1, z] + 1;
        }

        //
        // X MINESW DEBUG
        // 
        Debug.Log("Right Minesweeper:");
        var srm = "";
        for (int z = 0; z < lenZ; z++)
        {
            for (int x = 0; x < lenX; x++)
                srm += RightMinesweeper[x, z];
            srm += "\n";
        }
        Debug.LogWarning(srm);

        //
        // play down minesweeper
        // 
        for (int x = 0; x < lenX; x++)
        {
            DownMinesweeper[x, 0] = Blocked[x, 0] ? 0 : 1;
            for (int z = 1; z < lenZ; z++)
                DownMinesweeper[x, z] = Blocked[x, z] ? 0 : DownMinesweeper[x, z - 1] + 1;
        }

        BoxNa.Add(new Globox()); // 0 is trash

        // 
        // Y MINESW DEBUG
        //
        Debug.Log("Down Minesweeper:");
        var sdm = "";
        for (int z = 0; z < lenZ; z++)
        {
            for (int x = 0; x < lenX; x++)
                sdm += DownMinesweeper[x, z];
            sdm += "\n";
        }
        Debug.LogWarning(sdm);

        //
        // grow rectangles
        // 
        for (int z = lenZ - 1; z >= 0; z--)
        {
            for (int x = lenX - 1; x >= 0; x--)
            {
                if (CellOwnership[x, z] == 0)
                {
                    if (Blocked[x, z])
                        Debug.LogErrorFormat("considering blocked cell at {0},{1}", x, z);
                    // Unassigned! Make a new rect and start growing it.
                    var r = BoxNa.Count;
                    //CellOwnership[x, z] = r; // already covered below
                    var glo = new Globox();
                    
                    glo.Bounds.maxX = x;
                    glo.Bounds.maxZ = z;

                    // work out what the minesweeper value SHOULD be if this thing wasn't utterly fucking broken
                    var mmx = x - RightMinesweeper[x, z] + 1;
                    var mmz = z - DownMinesweeper[x, z] + 1;

                    var minDms = DownMinesweeper[x, z];
                    for (int i = mmx; i < x; i++)
                        minDms = DownMinesweeper[i, z] < minDms ? DownMinesweeper[i, z] : minDms;
                    var minRms = RightMinesweeper[x, z];
                    for (int j = mmz; j < z; j++)
                        minRms = RightMinesweeper[x, j] < minRms ? RightMinesweeper[x, j] : minRms;

                    // so, we can now pick from either minRms&DownMinesweeper or minDms&RightMinesweeper
                    if (minDms * RightMinesweeper[x, z] > minRms * DownMinesweeper[x, z])
                    {
                        // grow rect up-left
                        glo.Bounds.minX = x - RightMinesweeper[x, z] + 1;
                        glo.Bounds.minZ = z - minDms + 1;
                    }
                    else
                    {
                        // grow rect up-left
                        glo.Bounds.minX = x - minRms + 1;
                        glo.Bounds.minZ = z - DownMinesweeper[x, z] + 1;
                    }

                    // mark cell ownership (possibly overwriting!)
                    for (int z2 = glo.Bounds.minZ; z2 <= glo.Bounds.maxZ; z2++)
                        for (int x2 = glo.Bounds.minX; x2 <= glo.Bounds.maxX; x2++)
                        {
                            CellOwnership[x2, z2] = r;
                            if (Blocked[x2, z2])
                                Debug.LogErrorFormat("overwrote cell at {0},{1} working from {2},{3}", x2, z2, x, z);
                        }

                    BoxNa.Add(glo);
                }
            }
        }

        // DBG CellOwnership
        var s = "";
        for (int z = 0; z < lenZ; z++)
        {
            for (int x = 0; x < lenX; x++)
                s += CellOwnership[x, z] == -1 ? "#" : CellOwnership[x, z].ToString();
            s += "\n";
        }
        Debug.LogWarning(s);

        //
        // find rectangle neighbours (( O(num rectangles^2) !! ))
        // 
        for (int i = 1; i < BoxNa.Count; i++)
        {
            var ib = BoxNa[i].Bounds;

            for (int j = 1; j < BoxNa.Count; j++)
            {    
                var jb = BoxNa[j].Bounds;

                //if (((ib.minX >= jb.minX && ib.minX <= jb.maxX) || (ib.minZ >= jb.minZ && ib.minZ <= jb.maxZ))
                    //&& ((ib.maxX ))

                var minxin = (ib.minX >= jb.minX && ib.minX <= jb.maxX);
                var minzin = (ib.minZ >= jb.minZ && ib.minZ <= jb.maxZ);
                var maxxin = (ib.maxX >= jb.minX && ib.maxX <= jb.maxX);
                var maxzin = (ib.maxZ >= jb.minZ && ib.maxZ <= jb.maxZ);

                if ((minxin || maxxin) && (minzin || maxzin))
                    BoxNa[j].Neighbours.Add(i);
            }
        }
    }

    public List<IntVector2> FindPath(IntVector2 src, IntVector2 dst)
    {
        var li = new List<IntVector2>();

        // IV2s are structs thank god
        src.x -= offX;
        src.z -= offZ;
        dst.x -= offX;
        dst.z -= offZ;

        // TODO: clamp & add post-op point if dst is outside collision area

        // is the dst in the same rectangle as the src?
        if (BoxNa[CellOwnership[src.x, src.z]].Bounds.Contains(dst.x, dst.z))
        {
            li.Add(new IntVector2(dst.x + offX, dst.z + offZ));
            return li;
        }

        // otherwise...
        Debug.LogFormat("src in rect {0}, dst in rect {1}", CellOwnership[src.x, src.z], CellOwnership[dst.x, dst.z]);

        // !!TEMP!!;
        
        li.Add(new IntVector2(dst.x + offX, dst.z + offZ));
        return li;
    }
}

