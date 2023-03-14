using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

//一旦申请停止后不可取消
[Serializable]
public class PauseHandler 
{
    public LinkedList<float> remains=new LinkedList<float>();
    [JsonIgnore]
    public bool IsPause{get{return Count>0;}set{}}
    public int Count=0;
    void CountAdder(int i)
    {
        Count+=i;
        Count=Mathf.Max(0,Count);
    }

    public void OnUpdate(float deltatime)
    {
        if(remains.Count<=0)return;
        LinkedListNode<float> curr=remains.First;
        while (curr!=null)
        {
            curr.Value-=deltatime;
            if(curr.Value<=0)
            {
                LinkedListNode<float> del=curr;
                curr=curr.Next;
                remains.Remove(del);
                CountAdder(-1);
            }
            else curr=curr.Next;
        }
    }
/// <summary>
/// 加pause计数并登记何时恢复
/// </summary>
/// <param name="s"></param>
    public void DoPauseFor(float s)
    {
        CountAdder(1);
        remains.AddLast(s);
    }
    /// <summary>
    /// 直接加减pause计数
    /// </summary>
    /// <param name="b"></param>
    public void DoPause(bool b){
        if(b)CountAdder(1);
        else CountAdder(-1);
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
    public static PauseHandler FromString(string s)
    {
        //PauseHandler temp=new PauseHandler();
       PauseHandler temp=JsonConvert.DeserializeObject<PauseHandler>(s);
        return temp;
    }
    
}
