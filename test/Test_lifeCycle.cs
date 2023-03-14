using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Test_lifeCycle : MonoBehaviour
{
    public GameObject o;
    // Start is called before the first frame update
    void Awake()
    {
        EventCenter.WorldCenter = GetComponent<EventCenter>();
        EventCenter.WorldCenter.Init();
        //playerSerializeTest();
        GameObject g = GameObject.Find("canvasGroup");
        g.GetComponent<UICenter>().Init();
    }
    Ticker tk = new Ticker(1);
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            o.SetActive(!o.activeInHierarchy);
        }
    }
    void tickerTest()
    {
        if (tk.IsReady())
            Debug.Log(Time.time);
    }
    void playerSerializeTest()
    {
        //PlayerData pd =new PlayerData()
    }

    void npcSerializeTest()
    {
        NpcData nd = new NpcData();

        nd.char_data = new Charactor_Data();
        nd.char_skill = new Charactor_Skill_Data();
        nd.part = new Item[3];
        //nd.skin = new Item();

        nd.char_data.runSpeed = 20;
        nd.char_skill.bladeSkill = 5;

        string temp = JsonConvert.SerializeObject(nd,Formatting.Indented);
        Debug.Log(temp);
        NpcData nnd = JsonConvert.DeserializeObject<NpcData>(temp);
    }

    void floattest()
    {
        float a = 8f;
        a = a / 0.5f / 16;
        Debug.Log(a);
    }
    
    void dicTest()
    {
        Dictionary<int, Item> dd = new Dictionary<int, Item>();
        dd.Add(1, new Item(10));
        dd.Add(2, new Item(2));

        Item ii = dd[1];

        dd.Remove(1);

        //Debug.Log(ii.id);
    }
}
