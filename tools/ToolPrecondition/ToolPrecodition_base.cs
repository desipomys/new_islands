using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolPrecodition_base : Precondition_base
{
    // Start is called before the first frame update
    public virtual void OnToolInit(EventCenter holder,ToolNode_base bf){}
}
