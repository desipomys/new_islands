using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// 玩家、AI等任意可持有工具的实体的持有工具功能基类
/// </summary>
public class PlayerHand : Hand
{
    public Transform lefthand, righthand;
    public int secondToolIndex = -1;//>=0时代表双持中
    //Waiter pauseWaiter = new Waiter();
    ItemPageChangeParm dropparm=new ItemPageChangeParm();
    ItemPageChangeParm toolChgparm = new ItemPageChangeParm();
    //装备也可以放在tools中
    public override void OnEventRegist(EventCenter evc)
    {
        Debug.Log("hand 初始化");
        //Tools = new BaseTool[3];//等bp初始化完后获取item对应的basetool

        evc.RegistFunc<Transform>(nameof(PlayerEventName.getLeftHand), getLeftHand);
        base.OnEventRegist(evc);
        center.RegistFunc<Transform>(nameof(PlayerEventName.getMainHand), getMainHand);
        center.RegistFunc<Vector3>(nameof(PlayerEventName.getMainHandPos), ()=> { return mainHandPos; });
        center.RegistFunc<int>(nameof(PlayerEventName.getCurrentHandHolding), () => { return currentToolIndex; });
        center.ListenEvent<float>(nameof(PlayerEventName.setHandPauseInTime), (float t) => { handler.DoPauseFor(t); });

         /*try
         {
             for (int i = 0; i < Tools.Length; i++)
             {
                 //Tools[i] = EventCenter.WorldCenter.GetParm<BaseTool>(nameof(EventNames.GetBareHand));
                 Tools[i].OnInit(evc, Item.Empty);
                 
             }
         }
         catch (System.Exception)
         {
             for (int i = 0; i < Tools.Length; i++)
             {
                 Tools[i] = null;
                 //Tools[i].OnInit(evc, Item.Empty);
             }
         }*/
        Debug.Log(currentToolIndex+" at playerhand");
        //if(Tools[currentToolIndex]!=null)
          //  Tools[currentToolIndex].OnEquip(center);
    }
    public override void AfterEventRegist()
    {
        Init();
    }

    protected override void OnKey(KeyCodeKind keys, Dictionary<KeyCode, KeyCodeStat> stats)
    {//左键 使用1； 右键 使用2； 
        //如果不是换武器按键就转发给tool
        if (Tools == null) return;
        if (handler.IsPause) return;
        if (keys.Contain(KeyCodeKind.Q)&&stats[KeyCode.Q]==KeyCodeStat.down)//丢下当前武器
        {
            DropTool();
        }
        else if (keys.Contain(KeyCodeKind.Alpha1)&&stats[KeyCode.Alpha1]==KeyCodeStat.down)//换武器
        {
            ChangeTool(0);
        }
        else if (keys.Contain(KeyCodeKind.Alpha2) && stats[KeyCode.Alpha2] == KeyCodeStat.down)//
        {
            ChangeTool(1);
        }
        else if (keys.Contain(KeyCodeKind.Alpha3) && stats[KeyCode.Alpha3] == KeyCodeStat.down)//
        {
            ChangeTool(2);
        }
        else Tools[currentToolIndex].OnAnyKey(center, keys, stats);
    }
    void ChangeTool(int index)
    {
        if (index == currentToolIndex) return;
        if (index == secondToolIndex) return;
        //播动画,停止输入一段时间(直接evc发event);将手持区UI当前tool框上（hand转发到UI）

        Tools[currentToolIndex].UnEquip(center);
        Tools[index].OnEquip(center);
        currentToolIndex = index;
        //center.SendEvent<int, BaseTool>(nameof(PlayerEventName.onHandChgTool), index, Tools[index]);
    }
    void DropTool()
    {
        //发送丢弃武器事件，由backpack将当前格item丢弃
        dropparm.x = currentToolIndex;
        dropparm.page = -1;
        center.SendEvent<ItemPageChangeParm>(nameof(PlayerEventName.dropBPItemAt), dropparm);
    }
    public override void OnToolSendFloatToUI(string name, float value)
    {
        switch (name)
        {
            case "spread":
                EventCenter.WorldCenter.SendEvent<float>(nameof(PlayerEventName.onSpreadChg), value);
                break;
            default:
                break;
        }
    }
    public override void OnToolUpdateItem(Item item, BaseTool updateTool)
    {
        int ind = 0;
        for (int i = 0; i < Tools.Length; i++)
        {
            if(Tools[i]==updateTool)
            {
                ind = i;
                break;
            }
        }
        toolChgparm.item = item;
        toolChgparm.page = -1;
        toolChgparm.x = ind;
        toolChgparm.y = 0;
        toolChgparm.mode = ObjInPageChangeMode.set;
        center.SendEvent<ItemPageChangeParm>(nameof(PlayerEventName.setItem), toolChgparm);
    }

    public override void OnAnim(string parm)
    {
        BaseTool btd = GetNowHolding();
        btd.OnHolderEvent(center,PlayerEventName.onAnimationEvent,parm);
    }
    void GenToolObject(int x)
    {
        ToolObjs[x] = EventCenter.WorldCenter.GetParm<string, GameObject>(nameof(EventNames.GetModelByName), Tools[x].ModelName);

        ToolObjs[x].transform.SetParent(getMainHand());
        ToolObjs[x].transform.localPosition = Vector3.zero;
        ToolObjs[x].transform.localRotation = Quaternion.identity;
    }
    //当手持槽对应的item被改变时
    public override void OnHandItemPageChg(ItemPageChangeParm parm)
    {
        if (parm.page != -1) return;

        if (parm.x == currentToolIndex)//更新的是当前手持中格子
        {
            //Debug.Log("当前hand update " + parm.ToString());
            if (Tools[parm.x] == null || BaseTool.IsNullOrBareHand(Tools[parm.x]))//改变位置原来是空或是空手物体
            {
                //Debug.Log("当前为空hand update " + parm.ToString());
                if (!Item.IsNullOrEmpty(parm.item) && (EventCenter.WorldCenter.GetParm<Item, string>(nameof(EventNames.GetBaseToolNameByItem), parm.item)!=bareHandName))//如果改变不为空且不为空手物体
                {
                    //Debug.Log("当前为空且换的不为空hand update " + parm.ToString());
                    Tools[parm.x].UnEquip(center);
                    //Tools[parm.x].OnDispose(center);//让loader回收时调用
                    Destroy(ToolObjs[parm.x]);//通知loadertool回收basetool,
                    //回收tools[parm.x]

                    Tools[parm.x] = EventCenter.WorldCenter.GetParm<Item, BaseTool>(nameof(EventNames.GetBaseToolByItem), parm.item);//生成新tool基于parm

                    if (Tools[parm.x] == null)
                        Tools[parm.x] = EventCenter.WorldCenter.GetParm<string, BaseTool>(nameof(EventNames.GetBareHand), bareHandName);//生成barehand

                    GenToolObject(parm.x);

                    Tools[parm.x].OnInit(center, parm.item);
                    Tools[parm.x].OnEquip(center);

                    center.SendEvent<int, BaseTool>(nameof(PlayerEventName.onHandChgTool), currentToolIndex, Tools[currentToolIndex]);
                }
                else
                {
                    // Debug.Log("当前为空且换的为空hand update " + parm.ToString());
                }
            }
            else//改变位置当前是有且不为barehand的
            {

                if (Tools[parm.x].ComPare(parm.item))
                {

                    Tools[parm.x].OnItemChange(center, parm.item);//用Item更新当前tool
                }
                else
                {
                    //if(!Item.IsNullOrEmpty(parm.item) && EventCenter.WorldCenter.GetParm<Item, bool>("IsBareHand", parm.item))
                    //Debug.Log("当前不为空手，hand update " + parm.ToString());
                    Tools[parm.x].UnEquip(center);
                    //Tools[parm.x].OnDispose(center);//让loader回收时调用
                    Destroy(ToolObjs[parm.x]);//通知loadertool回收basetool,
                    //回收tools[parm.x]

                    Tools[parm.x] = EventCenter.WorldCenter.GetParm<Item, BaseTool>(nameof(EventNames.GetBaseToolByItem), parm.item);//生成新tool基于parm
                    if(Tools[parm.x] ==null)
                        Tools[parm.x] = EventCenter.WorldCenter.GetParm<string, BaseTool>(nameof(EventNames.GetBareHand), bareHandName);//生成barehand

                    GenToolObject(parm.x);

                    Tools[parm.x].OnInit(center, parm.item);
                    Tools[parm.x].OnEquip(center);

                    center.SendEvent<int, BaseTool>(nameof(PlayerEventName.onHandChgTool), currentToolIndex, Tools[currentToolIndex]);

                }
            }

        }
        else//当前改变的不是手持格
        {
            if (Tools[parm.x] == null || BaseTool.IsNullOrBareHand(Tools[parm.x]))//改变位置原来是空或是空手物体
            {
                if (!Item.IsNullOrEmpty(parm.item) && (EventCenter.WorldCenter.GetParm<Item, string>(nameof(EventNames.GetBaseToolNameByItem), parm.item) != bareHandName))//如果改变不为空且不为空手物体
                {
                    //Tools[parm.x].UnEquip(center);
                    Destroy(ToolObjs[parm.x]);//通知loadertool回收basetool,
                    //回收tools[parm.x]

                    Tools[parm.x] = EventCenter.WorldCenter.GetParm<Item, BaseTool>(nameof(EventNames.GetBaseToolByItem), parm.item);//生成新tool基于parm

                    if (Tools[parm.x] == null)
                        Tools[parm.x] = EventCenter.WorldCenter.GetParm<string, BaseTool>(nameof(EventNames.GetBareHand), bareHandName);//生成barehand

                    GenToolObject(parm.x);

                    Tools[parm.x].OnInit(center, parm.item);
                    //Tools[parm.x].OnEquip(center);
                }

            }
            else//改变位置当前是有且不为barehand的
            {
                if (Tools[parm.x].ComPare(parm.item))
                {

                    Tools[parm.x].OnItemChange(center, parm.item);//用Item更新当前tool
                }
                else
                {
                    //if(!Item.IsNullOrEmpty(parm.item) && EventCenter.WorldCenter.GetParm<Item, bool>("IsBareHand", parm.item))
                    {
                        Tools[parm.x].UnEquip(center);
                        Destroy(ToolObjs[parm.x]);//通知loadertool回收basetool,
                                                  //回收tools[parm.x]

                        Tools[parm.x] = EventCenter.WorldCenter.GetParm<Item, BaseTool>(nameof(EventNames.GetBaseToolByItem), parm.item);//生成barehand

                        if (Tools[parm.x] == null)
                            Tools[parm.x] = EventCenter.WorldCenter.GetParm<string, BaseTool>(nameof(EventNames.GetBareHand), bareHandName);//生成barehand

                        GenToolObject(parm.x);

                        Tools[parm.x].OnInit(center, parm.item);
                        //Tools[parm.x].OnEquip(center);
                    }
                }
            }

        }
    }
    public override Transform getMainHand()
    {
        return righthand;
    }
    public Transform getLeftHand()
    {
        return lefthand;
    }
    public void LateUpdate()
    {
        mainHandPos = righthand.position;
    }

    #region 检测按键
    int IsDoubleHoldKey(KeyCodeKind keys, Dictionary<KeyCode, KeyCodeStat> stats)
    {
        if(keys.Contain(KeyCodeKind.LeftControl)
            && (stats[KeyCode.LeftControl]== KeyCodeStat.down|| stats[KeyCode.LeftControl] == KeyCodeStat.keep) //按下ctrl
            && (keys.Contain(KeyCodeKind.Alpha1)|| keys.Contain(KeyCodeKind.Alpha2)|| keys.Contain(KeyCodeKind.Alpha3)|| keys.Contain(KeyCodeKind.Alpha4)))
        {
            if (stats[KeyCode.Alpha1] == KeyCodeStat.down || stats[KeyCode.Alpha1] == KeyCodeStat.keep)
            {
                //当前手持物需与0号位形成双持
                return 0;
            }
            else if (stats[KeyCode.Alpha2] == KeyCodeStat.down || stats[KeyCode.Alpha2] == KeyCodeStat.keep)
            {
                //当前手持物需与1号位形成双持
                return 1;
            }
            else if (stats[KeyCode.Alpha2] == KeyCodeStat.down || stats[KeyCode.Alpha2] == KeyCodeStat.keep)
            {
                //当前手持物需与2号位形成双持
                return 2;
            }
            else return -1;
        }
        return -1;
    }

    #endregion

    #region 序列化
    public override void FromObject(object str)
    {
        Debug.Log("反序列化"+GetDataCollectPrio);
        JArray j = (JArray)str;
        currentToolIndex= j[0].ToObject<int>();
        secondToolIndex = j[1].ToObject<int>();
        Init();
        //StartCoroutine(waitBPFromObject());
        //从背包获取手持处item
    }
    public override object ToObject()
    {
        return new int[2] { currentToolIndex,secondToolIndex};
    }
    public void Init()
    {
        Item[] t = center.GetParm<int, Item[]>(nameof(PlayerEventName.Getbp_Items), -1);//需要等backpack的fromobject执行后才执行
        Tools = new BaseTool[t.Length];
        for (int i = 0; i < t.Length; i++)
        {

            if (Tools[i] != null)
            {
                if (i == currentToolIndex) { Tools[i].UnEquip(center); }
                Tools[i].OnDispose(center);
            }
            BaseTool bt = EventCenter.WorldCenter.GetParm<Item, BaseTool>(nameof(EventNames.GetBaseToolByItem), t[i]);
            if(bt==null)
                bt= EventCenter.WorldCenter.GetParm<string, BaseTool>(nameof(EventNames.GetBareHand),bareHandName);

            Tools[i] = bt;
            Tools[i].OnInit(center, t[i]);
            if (i == currentToolIndex)
            {
                Tools[i].OnEquip(center);

                center.SendEvent<int, BaseTool>(nameof(PlayerEventName.onHandChgTool), currentToolIndex, Tools[currentToolIndex]);
            }
        }
    }

    #endregion
    IEnumerator waitBPFromObject()
    {
        yield return null;
        
    }

    /// <summary>
    /// 当前是否双持状态
    /// </summary>
    /// <returns></returns>
    public bool IsTwoHanded()
    {
        return secondToolIndex >= 0;
    }
}
