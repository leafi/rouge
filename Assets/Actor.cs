using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Actor : AOCommon
{
    public float MoveSpeed;
    private Vector3? moveTarget = null;

    private Vector3 nextMove;

    private IntVector2 position;
    private bool mustRecalculatePath;
    private Action mustRecalculatePathAction;

    public event Action<IntVector2, IntVector2> OnPreGridMove;
    public event Action<IntVector2, IntVector2> OnPostGridMove;

	public virtual void Start()
    {
        Grid.Get().AddActor(this);
        position.x = Mathf.RoundToInt(this.transform.position.x);
        position.z = Mathf.RoundToInt(this.transform.position.z);
	}

    public Actor() : base()
    {
        mustRecalculatePathAction = new Action(() => mustRecalculatePath = true);
        Tick.Post.Add(mustRecalculatePathAction);
    }

    ~Actor()
    {
        Tick.Post.Remove(mustRecalculatePathAction);
    }
	
	public virtual void Update()
    {
        var nextPositionF = updateMoveTarget();

        if (position.x != Mathf.RoundToInt(nextPositionF.x) || position.z != Mathf.RoundToInt(nextPositionF.z))
        {
            IntVector2 nextPosition = new IntVector2(Mathf.RoundToInt(nextPositionF.x), Mathf.RoundToInt(nextPositionF.z));

            if (OnPreGridMove != null)
                OnPreGridMove(position, nextPosition);

            transform.position = nextPositionF;

            if (OnPostGridMove != null)
                OnPostGridMove(position, nextPosition);

            position = nextPosition;
        }
        else
            transform.position = nextPositionF;
	}

    private void recalculatePath()
    {
        //Debug.LogFormat("{0} -> {1}", transform.position.ToIntVector2().ToString(), moveTarget.Value.ToIntVector2().ToString());
        var l = Rayman.Get().FindPath(transform.position.ToIntVector2(), moveTarget.Value.ToIntVector2());
        //Debug.LogWarningFormat("p_len: {0}", l.Count);
        var np = l[0];
        nextMove = new Vector3(np.x, transform.position.y, np.z);
        mustRecalculatePath = false;
    }

    private Vector3 updateMoveTarget()
    {
        if (moveTarget.HasValue)
        {
            var nm = nextMove;
            var speed = MoveSpeed * Time.smoothDeltaTime;

            if ((moveTarget.Value - transform.position).sqrMagnitude <= speed * speed)
            {
                // Done.
                moveTarget = null;
            }
            else if (mustRecalculatePath || (nextMove - transform.position).sqrMagnitude <= speed * speed)
            {
                // Close enough to point. Need to recalc.
                recalculatePath();
            }

            var v = Vector3.MoveTowards(transform.position, nextMove, speed);
            //Debug.LogFormat("jj {0} -> kk {1}", transform.position, v);
            return v;
        }

        return transform.position;
    }

    public void MoveTo(IntVector2 iv2) { MoveTo(iv2.x, iv2.z); }
    public void MoveTo(int gridX, int gridZ)
    {
        moveTarget = new Vector3(gridX, transform.position.y, gridZ);
        mustRecalculatePath = true;

        /*if (!Grid.Get().IsBlocked(gridX, gridZ))
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
        }*/
    }

    public override void Load(Nini.Config.IConfig sav)
    {
        //
        // TODO: update for multipath // update for pre-post tick?
        //

        base.Load(sav);
        MoveSpeed = sav.GetFloat("Actor_MoveSpeed");
        if (sav.GetBoolean("Actor_moveTargetHasValue"))
            moveTarget = sav.GetVector3("Actor_moveTargetValue");
        else
            moveTarget = null;
        if (moveTarget.HasValue)
            mustRecalculatePath = true;
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
