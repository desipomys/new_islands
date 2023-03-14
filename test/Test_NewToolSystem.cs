using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;

public class Test_NewToolSystem : MonoBehaviour,IEventRegister
{
    BaseTool driver;
    //ToolDataContainer container;
    EventCenter center;

    public void AfterEventRegist()
    {
        
    }

    public void OnEventRegist(EventCenter e)
    {
        center = e;
        driver = GetComponent<BaseTool>();
        //container = GetComponent<ToolDataContainer>();
    }

    /// <summary>
    /// 测试内容：响应按键发射子弹，换弹
    /// 发子弹中途更改特效名等变量
    /// 
    /// </summary>

    private void Awake()
    {
        
    }
    void Start()
    {
        ToolEngine te = Resources.Load<ToolEngine>("SC/TOOL/toolEngine/t1");
        string s = JsonConvert.SerializeObject(te,JsonSetting.serializerSettings);
        Debug.Log(s);

        ToolEngine te2 = JsonConvert.DeserializeObject<ToolEngine>(s, JsonSetting.serializerSettings);
        AssetDatabase.CreateAsset(te2, "Assets/Resources/SC/TOOL/toolEngine/t2.asset");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
