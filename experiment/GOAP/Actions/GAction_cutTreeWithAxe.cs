using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;


public class GAction_cutTreeWithAxe:GoapAction {

    public GAction_cutTreeWithAxe() {
		addPrecondition ("equipAxe", true); // we need a tool to do this
		addPrecondition ("isPackFull", false); // if we have logs we don't want more
		addEffect ("hasWoods", true);
	}

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        throw new NotImplementedException();
    }

    public override float GetCost()
    {
        Item axe=center.GetParm<Item>("getNowHandHold");
        //if()如果item是axe则判定其砍树等级，等级越高cost越低
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