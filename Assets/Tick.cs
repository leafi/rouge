using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Events that trigger upon turn.
/// </summary>
public static class Tick
{
    public static List<Action> OncePost = new List<Action>();
    public static List<Action> Pre = new List<Action>();
    public static List<Action> Post = new List<Action>();

    internal static void Init()
    {
        Rayman.Rebuild();
    }

    internal static void NextPre()
    {
        foreach (var a in Pre)
            a();


    }

    internal static void NextPost()
    {
        Rayman.Rebuild();

        var op = OncePost.ToArray();
        OncePost.Clear();
        foreach (var a in op)
            a();

        foreach (var a in Post)
            a();

        Debug.Log("Turn.");
    }
}

