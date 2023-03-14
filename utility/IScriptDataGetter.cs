using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//可以给item等游戏基础类型继承，也可以用mono继承放到玩家等gobj上
public interface IScriptDataGetter
{
    object GetScriptData(int target,int parm);
    void SetScriptData(int target,int parm,object o);
}
public interface IScriptFunctionCaller
{
    void Call(int target,int target2,object[] parm);
}