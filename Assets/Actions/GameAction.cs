using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class GameAction
{
    public virtual bool Enabled { get { return true; } }
    public abstract string GetName(Actor doer, AOCommon on);
    public abstract void Do(Actor doer, AOCommon on);
}
