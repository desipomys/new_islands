using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolEffector_ChgEntityData : Effecttor_base
{
    public CharacterEventName[] datas;
    public float[] values;

    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

   

    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        ToolEngine te = (ToolEngine)buff;
        for (int j = 0; j < target.Length; j++)
        {
            CharacterEntity ent = target[j].GetComponent<CharacterEntity>();
            ent.ChangeData(datas, values);
        }
        

        te.Next();
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}
