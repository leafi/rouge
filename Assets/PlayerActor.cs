using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerActor : Actor
{
    public int HP = 100;
    public int MP = 20;
    public bool Asleep = false;
    public int Dazed = 0;

    private static PlayerActor pa;
    public static PlayerActor Get() { return pa; }

    public override IntBounds IntBounds
    {
        get
        {
            return new IntBounds(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z), Mathf.RoundToInt(transform.position.x) + 1, Mathf.RoundToInt(transform.position.z) + 1);
        }
    }

    public PlayerActor()
    {
        OnPreGridMove += delegate { Tick.NextPre(); };
        OnPostGridMove += delegate { Tick.NextPost(); };
    }

    public override void Start()
    {
        base.Start();
        Tick.Init();
        pa = this;
    }

    public override void Load(Nini.Config.IConfig sav)
    {
        base.Load(sav);
        HP = sav.GetInt("PlayerActor_HP");
        MP = sav.GetInt("PlayerActor_MP");
        Asleep = sav.GetBoolean("PlayerActor_Asleep");
        Dazed = sav.GetInt("PlayerActor_Dazed");
    }

    public override void Save(Nini.Config.IConfig sav)
    {
        base.Save(sav);
        sav.Set("PlayerActor_HP", HP);
        sav.Set("PlayerActor_MP", MP);
        sav.Set("PlayerActor_Asleep", Asleep);
        sav.Set("PlayerActor_Dazed", Dazed);
    }
}
