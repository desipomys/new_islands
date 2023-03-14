using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;


public class GAction_findWoodWithNoAxe:GoapAction {

    public GAction_findWoodWithNoAxe () {
		//addPrecondition ("equipAxe", true); // we need a tool to do this
		addPrecondition ("isPackFull", false); // if we have logs we don't want more
		addEffect ("hasWoods", true);
	}

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        throw new NotImplementedException();
    }

    public override float GetCost()
    {
        ///寻找最近的掉落物wood,木板，木棍
        //移动到目标并拾取
        return 0;
    }

    public override bool isDone ()
	{
		return true;
	}

    public override bool perform(GameObject agent)
    {
        throw new NotImplementedException();
    }

    public override bool requiresInRange ()
	{
		return true; // yes we need to be near a tree
	}

    public override void reset()
    {
        throw new NotImplementedException();
    }
}