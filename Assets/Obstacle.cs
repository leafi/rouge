using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Obstacle : AOCommon
{
    public bool StartInvisible;

	void Start()
    {
        if (StartInvisible)
            GetComponent<MeshRenderer>().enabled = false;

        Grid.Get().AddObstacle(this);
	}
	
	void Update()
    {
	    
	}
}
