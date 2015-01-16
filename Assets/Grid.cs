using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public struct IntBounds
{
    public int minX;
    public int minZ;
    public int maxX;
    public int maxZ;

    public IntBounds(int mx, int mz, int Mx, int Mz) { minX = mx; minZ = mz; maxX = Mx; maxZ = Mz; }

    public bool Contains(int gx, int gz)
    {
        return gx >= minX && gz >= minZ && gx <= maxX && gz <= maxZ;
    }
}

public struct IntVector2
{
    public int x;
    public int z;

    public IntVector2(int x, int z) { this.x = x; this.z = z; }
}

public class Grid : MonoBehaviour
{
    // TODO: 2d octree
    // (and actually maintain some sort of grid, rather than asking every object all the time)

    [HideInInspector]
    public List<Obstacle> Obstacles = new List<Obstacle>();
    [HideInInspector]
    public List<Actor> Actors = new List<Actor>();

    private static Grid g = null;
    public static Grid Get() { return g; }

    private float gridHeight;

    public void AddObstacle(Obstacle o)
    {
        Obstacles.Add(o);
    }

    public void AddActor(Actor a)
    {
        Actors.Add(a);
    }

    public IntVector2 GetCellFromMousePicker()
    {
        return GetCellFromRay(Camera.main.ScreenPointToRay(Input.mousePosition));
    }

    public IntVector2 GetCellFromRay(Ray r)
    {
        if (r.direction.y == 0)
        {
            if (r.origin.y - 0.05 < gridHeight && r.origin.y + 0.05 > gridHeight)
                return new IntVector2(Mathf.RoundToInt(r.origin.x), Mathf.RoundToInt(r.origin.z));
            else
                throw new Exception("Grid.GetCellFromRay: Ray has no y direction & y origin isn't on the grid");
        }
        else
        {
            var d = r.direction;
            d.Normalize();

            if (r.origin.y < gridHeight && d.y < 0 || r.origin.y > gridHeight && d.y > 0)
            {
                d.x = -d.x;
                d.y = -d.y;
                d.z = -d.z;
            }

            float times = (gridHeight - r.origin.y) / d.y;
            return new IntVector2(Mathf.RoundToInt(r.origin.x + times * d.x), Mathf.RoundToInt(r.origin.z + times * d.z));
        }
    }

    public bool IsBlocked(IntVector2 iv2) { return IsBlocked(iv2.x, iv2.z); }
    public bool IsBlocked(int gx, int gz)
    {
        return Obstacles.Any((o) => o.IntBounds.Contains(gx, gz));
    }

    public static IntBounds CalculateIntBounds(Mesh mesh, Transform transform)
    {
        var min = transform.TransformPoint(mesh.bounds.min);
        var max = transform.TransformPoint(mesh.bounds.max);
        var mx = min.x < max.x ? Mathf.RoundToInt(min.x) : Mathf.RoundToInt(max.x);
        var mz = min.z < max.z ? Mathf.RoundToInt(min.z) : Mathf.RoundToInt(max.z);
        var Mx = max.x < min.x ? Mathf.RoundToInt(max.x) : Mathf.RoundToInt(min.x);
        var Mz = max.z < min.z ? Mathf.RoundToInt(max.z) : Mathf.RoundToInt(min.z);

        // min size 1x1 (which takes up 4 grid points, but whatever)
        if (Mx - mx < 1)
            Mx++;
        if (Mz - mz < 1)
            Mz++;

        return new IntBounds(Mathf.RoundToInt(min.x), Mathf.RoundToInt(min.z), Mathf.RoundToInt(max.x), Mathf.RoundToInt(max.z));
    }

    public IEnumerable<Actor> FindActors(int gx, int gz)
    {
        return Actors.Where((a) => a.IntBounds.Contains(gx, gz));
    }

    public IEnumerable<Obstacle> FindObstacles(int gx, int gz)
    {
        return Obstacles.Where((o) => o.IntBounds.Contains(gx, gz));
    }

    public Grid()
    {
        g = this;
    }

	void Start()
    {
        Obstacles = new List<Obstacle>();
        Actors = new List<Actor>();
        gridHeight = transform.position.y; // TODO: this but better?
	}
	
	void Update()
    {
        g = this;
	}
}
