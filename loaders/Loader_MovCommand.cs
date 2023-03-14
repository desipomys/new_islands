using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Loader_MovCommand : BaseLoader
{
    string path = "SC/MOVSCRIPT/command/General";
    public Dictionary<string, MovScriptCommand> commands = new Dictionary<string, MovScriptCommand>();

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.RegistFunc<string, MovScriptCommand>(nameof(EventNames.GetMovCommandByStr), Get);
    }
    public override void OnLoaderInit(int prio)
    {
        base.OnLoaderInit(prio);
        if (prio != 0) return;

        read();
    }
    void read()
    {
        MovScriptCommand[] command = Resources.LoadAll<MovScriptCommand>(path);
        foreach (var item in command)
        {
            commands.Add(item.SimpleName, item);
        }
    }

    /// <summary>
    /// 应该创建一个新的movcommand
    /// </summary>
    /// <param name="nam"></param>
    /// <returns></returns>
    public MovScriptCommand Get(string nam)
    {
        string temp = JsonConvert.SerializeObject(commands[nam],JsonSetting.serializerSettings);
        return JsonConvert.DeserializeObject<MovScriptCommand>(temp,JsonSetting.serializerSettings);
    }
    
}
