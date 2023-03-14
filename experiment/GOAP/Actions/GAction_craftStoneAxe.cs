using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;


public class GAction_craftStoneAxe:GoapAction {
//有问题，目前每种物品的合成都要写一个新类

	//public Item targetCraftItem;
    public GAction_craftStoneAxe () {
		//addPrecondition ("hasCraftTableNearBy", true); // we need a tool to do this
		//addPrecondition ("hasItem", new Item(1));
		addPrecondition ("isPackFull", false); // if we have logs we don't want more
		addPrecondition ("hasStone", 1);//percondition的比较上要更加完善，要支持数字大小比较，目前只支持object的equal方法比较
		addPrecondition ("hasWoodStick", 1);
		addEffect ("hasAxe", true);
	}
    public override float GetCost()
    {
        //根据工作台距离计算移动耗时，与当前危险系数综合计算出cost
        return 0;
    }

    public override bool isDone ()
	{
		return true;
	}
	
	public override bool requiresInRange ()
	{
		return true; // yes we need to be near a tree
	}
	public override bool isInRange ()
	{//找最近的工作台
		return true;
	}

    public override void reset()
    {
        throw new NotImplementedException();
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        throw new NotImplementedException();
    }

    public override bool perform(GameObject agent)
    {
        throw new NotImplementedException();
    }
}
/*
public class GAction_craftSth:GoapAction {
//有问题，目前每种物品的合成都要写一个新类

	//public Item targetCraftItem;
    public GAction_craftSth (Item targetItem) {
		//addPrecondition ("hasCraftTableNearBy", true); // we need a tool to do this
		//addPrecondition ("hasItem", new Item(1));
		addPrecondition ("isPackFull", false); 

		CraftUnit cu=EventCenter.WorldCenter.GetParm<Item,CraftUnit>("GetCraftByProdu",targetCraftItem);
		for (int i = 0; i < cu.mats.Length; i++)
		{
			addPrecondition ("hasItem", cu.mats[i]);
		}
		addEffect ("hasItem", targetItem);
	}
    public override float GetCost()
    {
        ///寻找最近的工作台
        //移动到目标并拾取
    }

    public override bool isDone ()
	{
		return chopped;
	}
	
	public override bool requiresInRange ()
	{
		return true; // yes we need to be near a tree
	}
	public override bool isInRange ()
	{//找最近的工作台
		return true;
	}
}
*/