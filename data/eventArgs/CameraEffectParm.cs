using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CamaraEffectParm:BaseEventArg
{
    public float time;
    public float amplify;
    public DIR dir;
    public CameraShakeMode mode=CameraShakeMode.pingpong;


}