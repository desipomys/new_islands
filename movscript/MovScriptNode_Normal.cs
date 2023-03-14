using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[CreateAssetMenu(menuName = "MovScript/node/normal")]
public class MovScriptNode_Normal:MovScriptNode
{   
   

    [JsonProperty]
    public List<MovScriptCommand> Commands;

    public override int OnUpdate()
    {
        
        if(frontWait>FrontTimed){FrontTimed+=Time.deltaTime;return 1; }
        else 
        {
            //run
            try
            {
               for (int i = 0; i < Commands.Count; i++)
              {
                  Commands[i].Run(null,EventCenter.WorldCenter,hostEngine,null);
              }
              FrontTimed = 0;
              hostEngine.Next();
            }
            catch (System.Exception e)
            {
              RaiseError(e.Message);
              return -1;
            }
           
            return 0;
        }
       
    }

    public override void OnInit(MovScriptEngine eng)
    {
      foreach (var item in Commands)
      {
        item.OnEngineInit(this);
      }
    }
   
}