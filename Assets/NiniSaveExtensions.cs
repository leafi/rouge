using UnityEngine;

using Nini.Config;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class NiniSaveExtensions
{
    public static Vector2 GetVector2(this IConfig c, string k)
    {
        return new Vector2(c.GetFloat(k + "X"), c.GetFloat(k + "Y"));
    }

    public static void SetVector2(this IConfig c, string k, Vector2 v)
    {
        c.Set(k + "X", v.x);
        c.Set(k + "Y", v.y);
    }

    public static Vector3 GetVector3(this IConfig c, string k)
    {
        return new Vector3(c.GetFloat(k + "X"), c.GetFloat(k + "Y"), c.GetFloat(k + "Z"));
    }

    public static void SetVector3(this IConfig c, string k, Vector3 v)
    {
        c.Set(k + "X", v.x);
        c.Set(k + "Y", v.y);
        c.Set(k + "Z", v.z);
    }

    public static Quaternion GetQuaternion(this IConfig c, string k)
    {
        return new Quaternion(c.GetFloat(k + "QuaternionX"), c.GetFloat(k + "QuaternionY"), c.GetFloat(k + "QuaternionZ"), c.GetFloat(k + "QuaternionW"));
    }

    public static void SetQuaternion(this IConfig c, string k, Quaternion q)
    {
        c.Set(k + "QuaternionX", q.x);
        c.Set(k + "QuaternionY", q.y);
        c.Set(k + "QuaternionZ", q.z);
        c.Set(k + "QuaternionW", q.w);
    }

    public static void GetTransform(this IConfig c, string k, Transform outTransform)
    {
        outTransform.position = c.GetVector3(k + "Position");
        outTransform.rotation = c.GetQuaternion(k + "Rotation");
        outTransform.localScale = c.GetVector3(k + "LocalScale");
    }

    public static void SetTransform(this IConfig c, string k, Transform t)
    {
        c.SetVector3(k + "Position", t.position);
        c.SetQuaternion(k + "Rotation", t.rotation);
        c.SetVector3(k + "LocalScale", t.localScale);
    }

}
