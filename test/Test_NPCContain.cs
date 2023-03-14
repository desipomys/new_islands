using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_NPCContain : MonoBehaviour
{
    public int uiindex, targetindex;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            NpcData nd = new NpcData();
            nd.npcName = "≤‚ ‘";
            nd.profession = NpcProfession.Assaulter;
            nd.level = 1;


            EventCenter.WorldCenter.SendEvent<NpcData>(nameof(Container_PlayerData_EventNames.AddNPC),nd);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
           //uiindex,targetindex
            EventCenter.WorldCenter.SendEvent<int,int>(nameof(Container_PlayerData_EventNames.SetNPCSelectIndex), uiindex,targetindex);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            //uiindex,targetindex
            EventCenter.WorldCenter.SendEvent(nameof(Container_PlayerData_EventNames.AddNPCSlot));
        }
    }
}
