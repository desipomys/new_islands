//存取数据从gamemainmanager发起，经过这被分发到各个container
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using UnityEngine.SceneManagement;

public class ContainerManager:BaseManager
{
    //container不要自己去读文件
   static Dictionary<string,BaseContainer> containers=new Dictionary<string,BaseContainer>();
   public static T GetContainer<T>()where T: BaseContainer
   {
       BaseContainer bc;
       containers.TryGetValue(typeof(T).Name,out bc);
       return (T)bc;
   }


    public override void OnEventRegist(EventCenter e)//晚于loader初始化
    {//e是worldcenter
        Debug.Log("生命周期事件：containerinit");
        base.OnEventRegist(e);
        findClass();
        initContainer(e);//只初始化，先不加载
        //checkNeedLoad();//当前是ingame才需在这加载数据
    }
    public override void AfterEventRegist()
    {
        base.AfterEventRegist();
        foreach (var item in containers)
        {
            item.Value.AfterEventRegist();
        }
    
    }

    void findClass()//反射查找每一个继承BaseLoader的类实例化后加入字典
    {
        if(containers.Count>0)return;
        Assembly assembly = this.GetType().Assembly;
        Type[] types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.IsSubclassOf(typeof(BaseContainer)) && !type.IsAbstract)
                {
                BaseContainer bc = (BaseContainer)assembly.CreateInstance(type.Name);
                bc.OnEventRegist(center);
                   containers.Add(type.Name, bc);
                }
        }
        
    }
   void initContainer(EventCenter e)
   {
        foreach (KeyValuePair<string,BaseContainer> item in containers)
       {
           item.Value.OnEventRegist(e);
       }
        Debug.Log("container 事件注册阶段完成");
    }
   [Obsolete]
    void checkNeedLoad()
    {
        if(SceneManager.GetActiveScene().name=="ingame")
        {
            foreach (var item in containers)
            {
                item.Value.OnLoadSave(null);
            }
        }
    }

   public void OnLoadGame()
   {
        MapPrefabsData ingame = GetContainer<Container_MapPrefabData>().currentMap;
        foreach (var item in containers)//地形数据生成、读取阶段
        {
            item.Value.OnLoadGame(ingame,0);
        }
        foreach (var item in containers)//实体数据生成、读取阶段
        {
            item.Value.OnLoadGame(ingame, 1);
        }
        foreach (var item in containers)//玩家数据生成、读取阶段
        {
            item.Value.OnLoadGame(ingame, 2);
        }
        foreach (var item in containers)//movengine生成、读取+初始化阶段
        {
            item.Value.OnLoadGame(ingame, 3);
        }
        Debug.Log("container LoadGame阶段完成");

        //gameobj生成阶段
        foreach (var item in containers)//地形gameobj生成阶段
        {
            item.Value.OnBuildGame( 0);
        }
        foreach (var item in containers)//实体+玩家gameobj生成阶段
        {
            item.Value.OnBuildGame(1);
        }
        Debug.Log("container BuildGame阶段完成");
    }
    public void UnLoadGame()
    {
        foreach (var item in containers)
        {
            item.Value.UnLoadGame(0);
        }
        foreach (var item in containers)
        {
            item.Value.UnLoadGame(1);
        }
        foreach (var item in containers)
        {
            item.Value.UnLoadGame(2);
        }
        Debug.Log("container UnLoadGame阶段完成");
    }
   public void OnLoadSave(SaveData save)
   {
        //不止单独将savedata给container_savedata保存
        foreach (var item in containers)
        {
            item.Value.OnLoadSave(save);
        }
        Debug.Log("container LoadSave阶段完成");
        //GetContainer<Container_SaveData>().Load(save);
        /*if(save.isInGame)//在ingame存储,让gamemainmanager处理
        {
            if(FileSaver.CheckMapFolderAvalible(save.savePath))
            {
                //跳转ingame场景
                
                center.SendEvent<string>("JumpToScene",ConstantValue.ingameSceneName);

                //读取各种ingame下的文件给container,如entity,chunk等数据文件

            }
        }*/

    }
    public void UnLoadSave()
    {
        foreach (var item in containers)
        {
            item.Value.UnLoadSave();
        }
        Debug.Log("container UnLoadGame阶段完成");
    }
    public void Save(string path)
    {
        foreach (var item in containers)
        {
            item.Value.Save(path);
        }
        
    }
    public void OnArriveStartScene()
    {
        foreach (var item in containers)
        {
            item.Value.OnArriveStartScene();
        }
    }
    public void OnQuitStartScene()
    {
        foreach (var item in containers)
        {
            item.Value.OnQuitStartScene();
        }
    }
    public void OnArriveInGameScene()
    {
        foreach (var item in containers)
        {
            item.Value.OnArriveInGameScene();
        }
    }
    public void OnQuitInGameScene()
    {
        foreach (var item in containers)
        {
            item.Value.OnQuitInGameScene();
        }
    }

    private void Update()
    {
        foreach (var item in containers)
        {
            item.Value.OnUpdate();
        }
    }
}