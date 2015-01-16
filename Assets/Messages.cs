using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Messages : MonoBehaviour
{
    private static Messages m;
    public static Messages Get()
    {
        return m;
    }

	void Start()
    {
        m = this;
	}

    List<string> lines = new List<string>();

    public void AddMessageF(string fmt, params object[] args) { AddMessage(string.Format(fmt, args)); }
    public void AddMessage(string text)
    {
        lines.Add(text);
        GetComponent<Text>().text = string.Join("\n", lines.GetRange(Mathf.Max(lines.Count - 5, 0), Mathf.Min(lines.Count, 5)).ToArray());
    }

    public static void M(string fmt, params object[] args)
    {
        Get().AddMessageF(fmt, args);
    }
	
	void Update()
    {
	}
}
