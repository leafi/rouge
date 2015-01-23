using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Actor : AOCommon
{
    public float MoveSpeed;
    private Vector3? moveTarget = null;
    private List<Vector3> moveTargetList = new List<Vector3>();

	public virtual void Start()
    {
        Grid.Get().AddActor(this);
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
            {
                // TODO: recalc!!!!!
                if (moveTargetList.Count == 0)
                    moveTarget = null;
                else
                {
                    moveTarget = moveTargetList[0];
                    moveTargetList.RemoveAt(0);
                    mt = moveTarget.Value;
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, mt, speed);
        }
    }

    public void MoveTo(IntVector2 iv2) { MoveTo(iv2.x, iv2.z); }
    public void MoveTo(int gridX, int gridZ)
    {
        // TODO: pathfind!

        if (!Grid.Get().IsBlocked(gridX, gridZ))
        {
            //moveTarget = new Vector3(gridX, transform.position.y, gridZ);
            // TODO: better src!!!
            var li = new Rayman().FindPath(new IntVector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)), new IntVector2(gridX, gridZ));
            moveTargetList = li.ToList().ConvertAll<Vector3>((iv2) => new Vector3(iv2.x, transform.position.y, iv2.z));
            moveTarget = moveTargetList[0];
            moveTargetList.RemoveAt(0);
        }
        else
        {
            Messages.M("That path is blocked.");
        }
    }

    public override void Load(Nini.Config.IConfig sav)
    {
        //
        // TODO: update for multipath
        //

        base.Load(sav);
        MoveSpeed = sav.GetFloat("Actor_MoveSpeed");
        if (sav.GetBoolean("Actor_moveTargetHasValue"))
            moveTarget = sav.GetVector3("Actor_moveTargetValue");
        else
            moveTarget = null;
    }

    public override void Save(Nini.Config.IConfig sav)
    {
        //
        // TODO: update for multipath
        //

        base.Save(sav);
        sav.Set("Actor_MoveSpeed", MoveSpeed);
        sav.Set("Actor_moveTargetHasValue", moveTarget.HasValue);
        if (moveTarget.HasValue)
            sav.SetVector3("Actor_moveTargetValue", moveTarget.Value);
    }
}
