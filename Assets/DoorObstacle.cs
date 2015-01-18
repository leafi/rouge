using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DoorObstacle : Obstacle
{
	public override void Start()
    {
        base.Start();
        Actions.Add(new DoorOpenAction());
	}
}
