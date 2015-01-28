using UnityEngine;
using UnityEngine.UI;

using Nini.Config;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SimpleMonsterActor : Actor
{
    float timeSinceLastMove = 0;

	public override void Start()
    {
	    
	}
	
	public override void Update()
    {
	    if (!this.moveTarget.HasValue)
        {
            timeSinceLastMove += Time.deltaTime;

            if (timeSinceLastMove > 2f)
            {
                var currentPos = transform.position.ToIntVector2();
                var gridTarget = new IntVector2(currentPos.x + UnityEngine.Random.Range(-2, 2), currentPos.z + UnityEngine.Random.Range(-2, 2));
                this.MoveTo(gridTarget);
                timeSinceLastMove = 0f;
            }
        }
	}
}
