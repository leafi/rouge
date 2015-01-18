using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Actor : AOCommon
{
    public float MoveSpeed;
    private Vector3? moveTarget = null;

	public virtual void Start()
    {

	}
	
	public virtual void Update()
    {
        updateMoveTarget();
	}

    private void updateMoveTarget()
    {
        if (moveTarget.HasValue)
        {
            var mt = moveTarget.Value;
            var speed = MoveSpeed * Time.smoothDeltaTime;

            if ((mt - transform.position).sqrMagnitude <= speed * speed)
                moveTarget = null;

            transform.position = Vector3.MoveTowards(transform.position, mt, speed);
        }
    }

    public void MoveTo(IntVector2 iv2) { MoveTo(iv2.x, iv2.z); }
    public void MoveTo(int gridX, int gridZ)
    {
        // TODO: pathfind!

        if (!Grid.Get().IsBlocked(gridX, gridZ))
        {
            moveTarget = new Vector3(gridX, transform.position.y, gridZ);
        }
        else
        {
            Messages.M("That path is blocked.");
        }
    }

    public override void Load(Nini.Config.IConfig sav)
    {
        base.Load(sav);
        MoveSpeed = sav.GetFloat("Actor_MoveSpeed");
        if (sav.GetBoolean("Actor_moveTargetHasValue"))
            moveTarget = sav.GetVector3("Actor_moveTargetValue");
        else
            moveTarget = null;
    }

    public override void Save(Nini.Config.IConfig sav)
    {
        base.Save(sav);
        sav.Set("Actor_MoveSpeed", MoveSpeed);
        sav.Set("Actor_moveTargetHasValue", moveTarget.HasValue);
        if (moveTarget.HasValue)
            sav.Set("Actor_moveTargetValue", moveTarget.Value);
    }
}
