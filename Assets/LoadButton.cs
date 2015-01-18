using UnityEngine;
using UnityEngine.UI;

using Nini.Config;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LoadButton : MonoBehaviour
{
	void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { GameSaveLoad.Load(); });
	}
	
	void Update()
    {
	    
	}
}
