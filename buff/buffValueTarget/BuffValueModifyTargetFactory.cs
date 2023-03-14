
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/// <summary>
/// 根据传入类型返回修改目标实例
/// </summary>
public class BuffValueModifyTargetFactory
{
   static Dictionary<BuffValueModifyTarget_Type, BuffValueModifyTarget_base> factory=new Dictionary<BuffValueModifyTarget_Type, BuffValueModifyTarget_base>();
   public static BuffValueModifyTarget_base GetInstance(BuffValueModifyTarget_Type type)
   {
        //Debug.Log(type);
      if(!factory.ContainsKey(type))append(type);
      return factory[type];
   }
    public static BuffValueModifyTarget_base GetInstance(BuffValueModifyTarget_base targ)
    {
        //Debug.Log(type);
        BuffValueModifyTarget_Type type = (BuffValueModifyTarget_Type)Enum.Parse(typeof(BuffValueModifyTarget_Type),targ.GetType().Name);
        return GetInstance(type);
    }
    static void append(BuffValueModifyTarget_Type type)
   {
      Assembly assembly = typeof(BuffValueModifyTargetFactory).Assembly;
        BuffValueModifyTarget_base temp = (BuffValueModifyTarget_base)assembly.CreateInstance(type.ToString());
     factory.Add(type,temp) ;
        //Debug.Log(type.ToString());
        //Debug.Log(temp == null ? "null" : temp.GetType().Name);
   }
  
}