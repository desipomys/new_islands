using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolEffector_SubAmmoItem : Effecttor_base
{
    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

   

    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        ToolEngine te = (ToolEngine)buff;
        //throw new System.NotImplementedException();
        //���ݵ�ǰsubid��max����ȷ���۶��ٵ�ҩitem
        //���ݵ�ҩ��ת����͵�ǰ��ҩindexȷ������Щ��ҩ
        //���سɹ��۳�����������ӵ�subid

        te.Next();
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}
