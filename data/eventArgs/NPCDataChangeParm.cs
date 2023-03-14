using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDataChangeParm : BaseEventArg
{
    public int trueIndex;
    /// <summary>
    /// 其实与UI无关，代表的是选中出战的第几个npc
    /// </summary>
    public int UIindex=-1;
    public NpcData data;
    public ItemPageChangeParm bpchg=new ItemPageChangeParm();
    public NPCDataChgPOS pos;
    public ObjInPageChangeMode mode;
}
public enum NPCDataChgPOS
{
    none=0,
    chardata=1,//1
    skill=chardata<<1,//2
    bp=skill<<1,//4
    skin=bp<<1,//8
    equip=skin<<1,//16
    part=equip<<1,//32
    defaultItem=part<<1,//64
    hand=defaultItem<<1,//128

    all=255
}