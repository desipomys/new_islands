using System.Collections.Generic;
using UnityEngine;
using System.Collections;
//using System;


public class GAction_defendArea:GoapAction {

    public GAction_defendArea () {
		
		//addPrecondition ("canMove", true);
		addEffect ("defendArea", true);
	}
    public override float GetCost()
    {
        ///寻找最近的掉落物wood,木板，木棍
        //移动到目标并拾取
        return 0;
    }
	public override void reset() {
		float temp=sensor.GetData<float>("renavTime");
		ticker=new Ticker(temp);
	}
	Ticker ticker;
	public override bool perform(GameObject agent)
	{//已进入area范围
		if(ticker.IsReady())//每n秒随机移动一次
		{
			center.SendEvent<Vector3>("aiMoveTo",getrandommovepos());
		}
        return true;
	}
	Vector3 getrandommovepos()
	{
        Vector3 temp = sensor.GetData<Vector3>("areaSize");
		return sensor.GetData<Vector3>("areaPosi")+new Vector3(Random.Range(-temp.x/2,temp.x/2), Random.Range(-temp.y / 2, temp.y / 2),Random.Range(-temp.z / 2, temp.z / 2));
	}
    public override bool isDone ()
	{
		return true;
	}
	
	public override bool requiresInRange ()
	{
		return true; // yes we need to be near a tree
	}

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        return true;
    }

}