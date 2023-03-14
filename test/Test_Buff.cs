using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

public class Test_Buff : MonoBehaviour
{
    JsonSerializerSettings setting = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting= Formatting.Indented,
    };
    // Start is called before the first frame update
    void Start()
    {
        test5();
        //Debug.Log(newbuff.GetItem(100).GetVar<Item>("bp").id);
    }

    void test5()
    {
        BaseBuff cmd = Resources.Load<BaseBuff>("SC/BUFF/q");
        string s = JsonConvert.SerializeObject(cmd, setting);
        Debug.Log(s);

        BaseBuff cmd2 = JsonConvert.DeserializeObject<BaseBuff>(s, setting);
        ((BuffValueSource_direct)cmd2.datas[BuffEventPoint.OnInit][0].effectors[0].valueSource).data = 10;
        Debug.Log(JsonConvert.SerializeObject(cmd2, setting));

        Debug.Log(JsonConvert.SerializeObject(cmd, setting));
        Debug.Log(cmd2.datas[BuffEventPoint.OnInit][0].effectors[0].valueSource.GetType().Name);
        Debug.Log(((BuffValueSource_direct)cmd2.datas[BuffEventPoint.OnInit][0].effectors[0].valueSource).data.data);
    }
}
