using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

public class Test_movEngine : MonoBehaviour
{
    JsonSerializerSettings setting = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting = Formatting.Indented,
    };
    public string s;
    // Start is called before the first frame update
    void Start()
    {
        test5();
        //Debug.Log(newbuff.GetItem(100).GetVar<Item>("bp").id);
    }
    MovScriptEngine cmd;
    void test5()
    {
        cmd = Resources.Load<MovScriptEngine>("SCObj/mov/eng/1");
        string s = JsonConvert.SerializeObject(cmd, setting);
        Debug.Log(s);

        //cmd.OnEnter(null);
       
        MovScriptEngine cmd2 = JsonConvert.DeserializeObject<MovScriptEngine>(s, setting);
       

    }

    public void Update()
    {
        //cmd.OnUpdate();
    }

}
