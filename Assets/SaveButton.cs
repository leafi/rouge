using UnityEngine;
using UnityEngine.UI;

using Nini.Config;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class SaveButton : MonoBehaviour
{
	void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { GameSaveLoad.Save(); });
	}
	
	void Update()
    {
	    
	}
}
