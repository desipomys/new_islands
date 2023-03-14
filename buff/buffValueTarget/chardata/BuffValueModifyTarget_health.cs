using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffValueModifyTarget_health : BuffValueModifyTarget_base
{
    public static BuffValueModifyTarget_health _instan;

    public override BuffValueModifyTarget_base GetInstance()
    {
        if (_instan == null) _instan = new BuffValueModifyTarget_health();
        return _instan;
    }

    public override FP valueGetter(EventCenter center)
    {
        FP temp = null;// center.GetComponent<CharacterEntity>().health;
       
        return temp;
    }

    public override void valueSetter(FP p, EventCenter center)
    {
        //Debug.Log("…Ë÷√Œ™" + p.Convert<float>());
        //center.GetComponent<CharacterEntity>().health= p.Convert<float>();  
    }

    public override Type valueType()
    {
        return typeof(float);
    }
}
public class BuffValueModifyTarget_Defend : BuffValueModifyTarget_base
{
    public static BuffValueModifyTarget_Defend _instan;

    public override BuffValueModifyTarget_base GetInstance()
    {
        if (_instan == null) _instan = new BuffValueModifyTarget_Defend();
        return _instan;
    }
    public override FP valueGetter(EventCenter center)
    {
        FP temp = null;//center.GetComponent<CharacterEntity>().defend;
       
        return temp;
    }

    public override void valueSetter(FP p, EventCenter center)
    {
        //center.GetComponent<CharacterEntity>().defend = p;
    }

    public override Type valueType()
    {
        return typeof(float);
    }
}

public class BuffValueModifyTarget_HealthUp : BuffValueModifyTarget_base
{
    public static BuffValueModifyTarget_HealthUp _instan;

    public override BuffValueModifyTarget_base GetInstance()
    {
        if (_instan == null) _instan = new BuffValueModifyTarget_HealthUp();
        return _instan;
    }
    public override FP valueGetter(EventCenter center)
    {
        FP temp = null;// center.GetComponent<CharacterEntity>().healthUp;
      
        return temp;
    }

    public override void valueSetter(FP p, EventCenter center)
    {
       // center.GetComponent<CharacterEntity>().healthUp = p;
    }

    public override Type valueType()
    {
        return typeof(float);
    }
}