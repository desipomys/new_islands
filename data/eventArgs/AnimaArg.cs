using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimaArg : BaseEventArg
{
    static AnimaArg arg;
    public static AnimaArg instance { get { if (arg == null) arg = new AnimaArg();return arg; } set { } }

    public string AnimName;
    public int layer;

}
