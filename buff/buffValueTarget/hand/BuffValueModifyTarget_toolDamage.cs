using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffValueModifyTarget_toolDamage : BuffValueModifyTarget_base
{
    public override BuffValueModifyTarget_base GetInstance()
    {
        return BuffValueModifyTargetFactory.GetInstance(this);
    }
    public override FP valueGetter(EventCenter center)
    {
        throw new System.NotImplementedException();
    }

    public override void valueSetter(FP p, EventCenter center)
    {
        throw new System.NotImplementedException();
    }

    public override Type valueType()
    {
        throw new NotImplementedException();
    }
}
