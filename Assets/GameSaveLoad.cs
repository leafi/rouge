using UnityEngine;

using Nini.Config;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class GameSaveLoad
{
    public static void Save()
    {
        var ini = new IniConfigSource();

        Grid.Get().Save(ini);
        GridPointer.Get().Save(ini.AddConfig("_GridPointer"));
        Messages.Get().Save(ini.AddConfig("_Messages"));
        PlayerCamera.Get().Save(ini.AddConfig("_PlayerCamera"));
        
        ini.Save("c:\\users\\leaf\\desktop\\rouge\\save.ini");

        Messages.M("Save complete.");
    }

    public static void Load()
    {
        var ini = new IniConfigSource("c:\\users\\leaf\\desktop\\rouge\\save.ini");

        PlayerCamera.Get().Load(ini.Configs["_PlayerCamera"]);
        Grid.Get().Load(ini);
        GridPointer.Get().Load(ini.Configs["_GridPointer"]);
        Messages.Get().Load(ini.Configs["_Messages"]);

        Messages.M("Load complete.");
    }
}
