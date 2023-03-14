using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayChangeEventArg<T>:BaseEventArg
{
    public int index;
    public T now,old;//变化后，最大，变化前
}

public class Array2DChangeEventArg<T>:BaseEventArg
{
    public int x,y;
    public T now,old;//变化后，最大，变化前
}
public class Array3DChangeEventArg<T>:BaseEventArg
{
    public int x,y,z;
    public T now,old;//变化后，最大，变化前
}