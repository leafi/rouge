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

    void Start()
    {
        pa = this;
    }
}
