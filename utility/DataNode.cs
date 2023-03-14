using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
public interface IDataNode
{
    string nam{get;}
    bool GetData<T>(string path,out T ans);
    bool SetData<T> (string path,T dat);
    bool SetOrAddData<T>(string path,T dat);
    void AddChild(IDataNode node);//不允许孩子节点重名
    void DeleteChildByName(string name);
    IDataNode GetChild(string name);
    /// <summary>
    /// 以序列化后dic（str,str）的形式返回数据
    /// </summary>
    /// <returns></returns>
    //string Serialize();
    
}

/// <summary>
/// 未测试
/// </summary>
public class DataNode :IDataNode
{
    public static readonly char[] splitor=new char[] { '.' };
    public string nam{ get; set; }
   
    public List<IDataNode> child=new List<IDataNode>();
     IDataNode parent; 
    public object data;

    public DataNode() { }
    public DataNode(string name){nam=name;}
    public DataNode(string name,object dat){nam=name;data=dat;}
    public bool GetData<T>(string path,out T ans)
    {   
        if(string.IsNullOrEmpty(path)){ans=(T)data;return true;}
        else
        {
            string[] paths=path.Split(splitor,System.StringSplitOptions.RemoveEmptyEntries);
            return GetData<T>(paths,out ans);
        }
    }
    public bool GetData<T>(string[] paths,out T ans)
    {   
       bool succ=true;
        IDataNode temp=this;
        for(int i=0;i<paths.Length;i++)
        {
            temp= temp.GetChild(paths[i]);
            if(temp==null){ans=default(T); return false;}
            if(i==paths.Length-1) return temp.GetData<T>("",out ans);
            else continue;
        }
        succ=false;
        ans=default(T);
        return succ;
    }

    public bool SetData<T>(string path,T dat)
    {   
        if(string.IsNullOrEmpty(path)){data=dat;return true;}
        else
        {
            string[] paths=path.Split(splitor, System.StringSplitOptions.RemoveEmptyEntries);
            return SetData<T>(paths,dat);
        }
    }
    public bool SetData<T>(string[] paths,T dat)
    {
        IDataNode temp=this;  
        for(int i=0;i<paths.Length;i++)
        {
            temp= temp.GetChild(paths[i]);
            if(temp==null)return false;
            if(i==paths.Length-1)
            {
                temp.SetData<T>(null,dat);
                return true;
            }
        }
        return false;
        
    }

    public void AddData<T>(string path,T dat)
    {
        if(string.IsNullOrEmpty(path)){return;}
        string[] paths=path.Split(splitor, System.StringSplitOptions.RemoveEmptyEntries);
        AddData<T>(paths,dat);
    }
    public void AddData<T>(string[] paths,T dat)
    {
        IDataNode root=this;
        IDataNode temp=this;  
        for(int i=0;i<paths.Length;i++)
        {
            temp= temp.GetChild(paths[i]);
            if(temp==null)
            {
                IDataNode n=new DataNode(paths[i]);
                root.AddChild(n);
                temp=n;
            }
            if(i==paths.Length-1)
            {
                temp.SetData<T>(null,dat);
                return;
            }
            root=temp;
        }
    }
    /// <summary>
    /// true为set,false为add
    /// </summary>
    /// <param name="path"></param>
    /// <param name="dat"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool SetOrAddData<T>(string path,T dat)
    {
        bool succ=SetData<T>(path,dat);
        if(!succ)
        {
            AddData<T>(path,dat);
        }  
        return succ;
    }
    public bool SetOrAddData<T>(string[] paths,T dat)
    {
        bool succ=SetData<T>(paths,dat);
        if(!succ)
        {
            AddData<T>(paths,dat);
        }  
        return succ;
    }

    public void AddChild(IDataNode node)//不允许孩子节点重名
    {
       for (int i = 0; i < child.Count; i++)
        {
            if(child[i].nam==node.nam)return;
        }
        ((DataNode)node).parent=this;
        child.Add(node);
    }
    public void DeleteChildByName(string name)
    {
       for (int i = 0; i < child.Count; i++)
        {
            if(child[i].nam==name){child.RemoveAt(i);return;}
        }
    }
    public IDataNode GetChild(string name)
    {
       for (int i = 0; i < child.Count; i++)
        {
            if(child[i].nam==name)return child[i];
        }
        return null;
    }


    public override string ToString()
    {
        List<string> datastr=new List<string>();
        datastr.Add(nam);
        datastr.Add(new DataWarper(data).ToString());
        List<string> temp=new List<string>();
        for(int i=0;i<child.Count;i++)
        {
            temp.Add(child[i].ToString());
        }
        datastr.Add(JsonConvert.SerializeObject(temp, Formatting.Indented));
        return JsonConvert.SerializeObject(datastr, Formatting.Indented);
    }
    public static DataNode FromString(string d)
    {
        List<string> datastr=JsonConvert.DeserializeObject<List<string>>(d);
        DataNode node=new DataNode(datastr[0]);
        node.data=DataWarper.FromString(datastr[1]).UnLoad();
        List<string> temp=JsonConvert.DeserializeObject<List<string>>(datastr[2]);
        node.child = new List<IDataNode>();
        for(int i=0;i<temp.Count;i++)
        {
            node.child.Add(DataNode.FromString(temp[i]));
        }
        return node;
    }
    public object ToObject()
    {
        JObject j=new JObject();
        j.Add("nam",nam);
        j.Add("data",JObject.CreateFromContent(data));
        if(data!=null)
            j.Add("typ",data.GetType().Name);
        JArray array=new JArray();
        foreach (var item in child)
        {
            array.Add(((DataNode)item).ToObject());
        }
        j.Add("child",array);
        return j;
    }
    public DataNode FromObject(object o)
    {
        JObject j=(JObject)o;
        //DataNode node=new DataNode();
        nam=j["nam"].ToString();
        if(j["typ"]!=null&&j["data"]!=null)
        {
            //Debug.Log(j["data"].ToObject<int>() +":"+j["typ"]);
            Type t=Type.GetType("System."+j["typ"].ToString());
            //if(t==null)Debug.Log(j["typ"].ToString()+"=null");
            data=j["data"].ToObject(t);
        }
        JArray array=(JArray)j["child"];
        if(array!=null)
            foreach (var item in array)
            {
                if(item!=null)
                   { 
                       DataNode t=new DataNode();
                       AddChild(t.FromObject((JObject)item));
                   }
            }
        //(DataNode)j.ToObject(Type.GetType(j["typ"].ToString()));
        return this;
    }

}
