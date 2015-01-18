using UnityEngine;

using Nini.Config;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class AOCommon : MonoBehaviour
{
    protected List<GameAction> gameActions = new List<GameAction>();
    public virtual List<GameAction> Actions { get { return gameActions; } }
    public string ResourcesPrefab = "";

    public virtual IntBounds IntBounds
    {
        get { return Grid.CalculateIntBounds(GetComponent<MeshFilter>().sharedMesh, transform); }
    }

    public static bool MagicInstantiate(IConfig sav)
    {
        if (sav.GetBoolean("__noMagicLoad", false))
            return false;

        if (sav.GetBoolean("__tryFindInScene"))
        {
            // Shit. We really do need to do something truly magical...
            var go = GameObject.Find(sav.GetString("__gameObjectName"));
            if (!go) // e.g. child object that hasn't been loaded yet
                return false;
            go.GetComponent<AOCommon>().Load(sav);
        }
        else
        {
            // Instantiate the prefab!
            Instantiate(Resources.Load<GameObject>(sav.GetString("__resourcesPrefab"))).GetComponent<AOCommon>().Load(sav);
        }

        return true;
    }

    public virtual void Load(IConfig sav)
    {
        ResourcesPrefab = sav.GetString("__resourcesPrefab", "");
        if (sav.Contains("__meshRendererEnabled"))
            GetComponent<MeshRenderer>().enabled = sav.GetBoolean("__meshRendererEnabled");
        sav.GetTransform("__transform", this.transform);
    }

    public virtual void Save(IConfig sav)
    {
        if (ResourcesPrefab == "")
            sav.Set("__tryFindInScene", true);
        else
        {
            sav.Set("__resourcesPrefab", ResourcesPrefab);
            sav.Set("__tryFindInScene", false);
        }
        sav.Set("__gameObjectName", this.gameObject.name);
        sav.Set("__instanceID", this.GetInstanceID());
        if (GetComponent<MeshRenderer>() != null)
            sav.Set("__meshRendererEnabled", GetComponent<MeshRenderer>().enabled);
        sav.SetTransform("__transform", this.transform);
    }
}
