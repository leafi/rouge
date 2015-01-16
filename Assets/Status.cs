using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Status : MonoBehaviour
{
    private static Status singleton;
    public static Status Get()
    {
        return singleton;
    }

    private string lastText = "not set";

    private void updateText(int hp, int mp, bool asleep, int dazed)
    {
        var text = string.Format("hp:{0} mp:{1} {2}{3}", hp, mp, asleep ? "Asleep " : "", dazed > 0 ? "dazed:" + dazed + "t" : "");
        if (text != lastText)
            GetComponent<Text>().text = text;
        lastText = text;
    }

	void Start()
    {
        singleton = this;
	}
	
	void Update()
    {
	    var pa = PlayerActor.Get();
        updateText(pa.HP, pa.MP, pa.Asleep, pa.Dazed);
	}
}
