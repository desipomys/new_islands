using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffValueModifyTarget_maxHealth : BuffValueModifyTarget_base
{
    public static BuffValueModifyTarget_maxHealth _instan;

    public override BuffValueModifyTarget_base GetInstance()
    {
        if (_instan == null) _instan = new BuffValueModifyTarget_maxHealth();
        return _instan;
    }
    public override FP valueGetter(EventCenter center)
    {
        FP temp = null;// center.GetComponent<CharacterEntity>().maxhealth;
       
        return temp;
    }

    public override void valueSetter(FP p, EventCenter center)
    {
        //center.GetComponent<CharacterEntity>().maxhealth = p;
    }

    public override Type valueType()
    {
        return typeof(float);
    }
}
