using UnityEngine;
using UnityEngine.UI;

using Nini.Config;

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

    public void AddMessageF(string fmt, params object[] args) { lines.Add(string.Format(fmt, args)); updateText(); }
    public void AddMessage(string text) { lines.Add(text); updateText(); }

    private void updateText()
    {
        GetComponent<Text>().text = string.Join("\n", lines.GetRange(Mathf.Max(lines.Count - 5, 0), Mathf.Min(lines.Count, 5)).ToArray());
    }

    public static void M(string fmt, params object[] args)
    {
        Get().AddMessageF(fmt, args);
    }
	
	void Update()
    {
	}

    public void Load(IConfig sav)
    {
        lines.Clear();
        for (int i = 0; i < sav.GetInt("linesCount"); i++)
            lines.Add(sav.GetString("lines" + i));
        updateText();
    }

    public void Save(IConfig sav)
    {
        sav.Set("linesCount", lines.Count);
        for (int i = 0; i < lines.Count; i++)
            sav.Set("lines" + i, lines[i]);
    }
}
