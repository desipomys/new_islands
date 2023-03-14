using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CallEffectParm:BaseEventArg
{
    public Vector3 pos;
    public Vector3 dir;
    public Vector3 speed;
    public float size;
    public string type;
    public Transform target;
    public float time;
    public object data;
}