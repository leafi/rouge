using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nini.Config;

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

    public override string ToString() { return "(" + minX + "," + minZ + ") -> (" + maxX + "," + maxZ + ")"; }
}

public struct IntVector2
{
    public int x;
    public int z;

    public IntVector2(int x, int z) { this.x = x; this.z = z; }
    public override string ToString() { return "(" + x + "," + z + ")"; }
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

    public static Dictionary<string, IConfig> AOLoadFailed = new Dictionary<string, IConfig>();

    private float gridHeight;

    public void AddObstacle(Obstacle o)
    {
        Obstacles.Add(o);
    }

    public void AddActor(Actor a)
    {
        Actors.Add(a);
    }

    public void Load(IConfigSource ini)
    {
        AOLoadFailed.Clear();

        foreach (IConfig c in ini.Configs)
        {
            if (c.Name.StartsWith("Actor_") || c.Name.StartsWith("Obstacle_"))
            {
                if (!AOCommon.MagicInstantiate(c))
                    AOLoadFailed.Add(c.Name, c);
            }
        }

        if (AOLoadFailed.Count > 0)
            Debug.LogWarning("Failed to load " + AOLoadFailed.Count + " Actor-Obstacles. (" + string.Join(", ", AOLoadFailed.Keys.ToArray()) + ")");
        
        Debug.Log("AO/grid load complete.");
    }

    public void Save(IConfigSource ini)
    {
        foreach (Actor a in Actors)
            a.Save(ini.AddConfig("Actor_" + a.GetInstanceID() + "_" + a.gameObject.name));
        foreach (Obstacle o in Obstacles)
            o.Save(ini.AddConfig("Obstacle_" + o.GetInstanceID() + "_" + o.gameObject.name));

        Debug.Log("Wrote, uh, some AO/grid load objects.");
    }

    public IntVector2 GetCellFromMousePicker()
    {
        return GetCellFromRay(Camera.main.ScreenPointToRay(Input.mousePosition));
    }

    public Vector2 GetFloatCellFromRay(Ray r)
    {
        if (r.direction.y == 0)
        {
            if (r.origin.y - 0.05 < gridHeight && r.origin.y + 0.05 > gridHeight)
                return new Vector2(r.origin.x, r.origin.z);
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
            return new Vector2(r.origin.x + times * d.x, r.origin.z + times * d.z);
        }
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
        return Obstacles.Any((o) => o.Blocking && o.IntBounds.Contains(gx, gz));
    }

    public static IntBounds CalculateIntBounds(Mesh mesh, Transform transform)
    {
        var min = transform.TransformPoint(mesh.bounds.min);
        var max = transform.TransformPoint(mesh.bounds.max);
        var mx = min.x < max.x ? Mathf.RoundToInt(min.x) : Mathf.RoundToInt(max.x);
        var mz = min.z < max.z ? Mathf.RoundToInt(min.z) : Mathf.RoundToInt(max.z);
        var Mx = max.x < min.x ? Mathf.RoundToInt(max.x) : Mathf.RoundToInt(min.x);
        var Mz = max.z < min.z ? Mathf.RoundToInt(max.z) : Mathf.RoundToInt(min.z);

        return new IntBounds(mx, mz, Mx, Mz);
    }

    public IEnumerable<Actor> FindActors(IntVector2 gv) { return FindActors(gv.x, gv.z); }
    public IEnumerable<Actor> FindActors(int gx, int gz)
    {
        return Actors.Where((a) => a.IntBounds.Contains(gx, gz));
    }

    public IEnumerable<Obstacle> FindObstacles(IntVector2 gv) { return FindObstacles(gv.x, gv.z); }
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
        gridHeight = transform.position.y; // TODO: this but better?
	}
	
	void Update()
    {
        g = this;
	}
}
