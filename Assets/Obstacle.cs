using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Obstacle : AOCommon
{
    public bool StartInvisible = false;
    public bool Blocking = true;

	public virtual void Start()
    {
        if (StartInvisible)
            GetComponent<MeshRenderer>().enabled = false;

        Grid.Get().AddObstacle(this);
	}
	
	public virtual void Update()
    {
	    
	}

    public override void Load(Nini.Config.IConfig sav)
    {
        base.Load(sav);
        StartInvisible = sav.GetBoolean("Obstacle_StartInvisible");
        Blocking = sav.GetBoolean("Obstacle_Blocking");

        // meshrenderer.enabled is handled by AOCommon. like one of the only things that is...
    }

    public override void Save(Nini.Config.IConfig sav)
    {
        base.Save(sav);
        sav.Set("Obstacle_StartInvisible", StartInvisible);
        sav.Set("Obstacle_Blocking", Blocking);
    }
}
