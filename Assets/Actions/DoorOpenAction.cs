using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DoorOpenAction : GameAction
{
    public override string GetName(Actor doer, AOCommon on)
    {
        return (on as Obstacle).Blocking ? "Open Door" : "Close Door";
    }

    public override void Do(Actor doer, AOCommon on)
    {
        (on as Obstacle).Blocking = !(on as Obstacle).Blocking;
        on.gameObject.GetComponent<MeshRenderer>().enabled = (on as Obstacle).Blocking;
    }
}
