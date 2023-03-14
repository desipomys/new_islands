using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Goap_Assalter: BaseGoapSensor  
{
     public override HashSet<KeyValuePair<string,object>> createGoalState ()
    {
        //defendarea 是goal,对应一个action,该action持续巡逻区域，遇敌则转killenemy goal，离太远转回defendarea goal
        HashSet<KeyValuePair<string,object>> goal = new HashSet<KeyValuePair<string,object>> ();
		
		goal.Add(new KeyValuePair<string, object>("defendArea", true ));
		return goal;
        
    }
}