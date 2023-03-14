using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueChangeParm<T>:BaseEventArg
{
    public T now,max,old;//变化后，最大，变化前
}