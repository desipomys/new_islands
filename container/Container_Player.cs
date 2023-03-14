using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
/// <summary>
/// 运行时所有玩家(本地+远程)数据的采集器
/// 不要放其他逻辑比如玩家死亡响应事件在这
/// </summary>
public class Container_Player : BaseContainer
{
    string fileName = "players.dat";
    string playerPrefabsPath = "Prefabs/player/player_test01";

    public Vector3 defalutSpawnPoint;
    long localPlayerIndex;
    Vector3 localPlayerOfflinePosi;

    Dictionary<long, EventCenter> players = new Dictionary<long, EventCenter>();//uuid-player
    Dictionary<long, string> tempPlayerData = new Dictionary<long, string>();

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.RegistFunc<Vector3[]>(nameof(EventNames.GetAllPlayerPosi), getAllPlayerPosi);
        e.ListenEvent<Vector3>(nameof(EventNames.SetDefaultSpawnPoint), SetDefaultSpawnPoint);
        e.RegistFunc< EventCenter>(nameof(EventNames.GetLocalPlayer),()=> {  return players[localPlayerIndex]; });
        e.RegistFunc<Vector3>(nameof(EventNames.GetDefaultSpawnPoint), () => { return defalutSpawnPoint; });
        e.ListenEvent<bool>(nameof(PlayerEventName.pause), onPause);
        e.ListenEvent<bool>(nameof(EventNames.PauseLocalPlayer), 
            (bool p) => {players[localPlayerIndex].SendEvent<bool>(nameof(PlayerEventName.pause),p); });
    }
    public override void OnLoadGame(MapPrefabsData data, int index)
    {
        if (index != 2) return;
        base.OnLoadGame(data, index);
        defalutSpawnPoint = Vector3.zero;//
        
        //如果player.txt为空则生成plaYER数据
        //否则直接读取，暂存在templayerdata中，后续buildgame时由genplayer方法使用
        try
        {
            string playerCharData = FileSaver.GetMapPlayerCharDatas(center.GetParm<string>(nameof(EventNames.ThisSavePath)));
            tempPlayerData = JsonConvert.DeserializeObject<Dictionary<long, string>>(playerCharData);
            Debug.Log("玩家数据加载成功，有" + (tempPlayerData.Count-2) + "个玩家来过");
            localPlayerIndex =long.Parse(tempPlayerData[-1]);
            localPlayerOfflinePosi=localPlayerOfflinePosi.FromString(tempPlayerData[-2]);
            //Debug.Log(tempPlayerData[-2]+"="+localPlayerOfflinePosi.ToString());
            center.RegistFunc<Vector3>(nameof(EventNames.GetLocalPlayerOfflinePosi),()=>{return localPlayerOfflinePosi;});//专供terraincontainer使用的获取玩家下线时地点
        }
        catch (System.Exception e)
        {
            Debug.Log("玩家数据加载失败" + e.Message + "。尝试生成玩家数据文件");
            try
            {
                FileSaver.SetMapPlayerCharDatas(center.GetParm<string>(nameof(EventNames.ThisSavePath)), "");
                tempPlayerData = new Dictionary<long, string>();
            }
            catch (System.Exception e1)
            {
                Debug.Log("玩家数据生成失败" + e1.Message);
            }
        }

    }
    public override void OnBuildGame(int index)
    {
        if (index == 1)
        {
            //生成玩家预设，设置其数据
            //
            //Vector3 v = Chunk.WorldPosToBlockPos(defalutSpawnPoint);
            defalutSpawnPoint.y = Chunk.WorldPosToBlockH(defalutSpawnPoint)+1;
            tempGenPlayer();
            center.ForceUnRegistFunc<Vector3>(nameof(EventNames.GetLocalPlayerOfflinePosi));
        }
    }
    public override void UnLoadGame(int ind)
    {
        if (ind != 0) return;
        base.UnLoadGame(ind);
        //Save(center.GetParm<string>(nameof(EventNames.ThisSavePath)));
        players.Clear();
        tempPlayerData.Clear();
        localPlayerIndex = 0;
    }
    public override void Save(string path)
    {
        bool avalible;
        if (center.GetParm<bool>(nameof(EventNames.IsInGame), out avalible))
        {
            if (avalible)
            {
                tempPlayerData.Clear();
                tempPlayerData.Add(-1, localPlayerIndex.ToString());//本地玩家index
                tempPlayerData.Add(-2, players[localPlayerIndex].gameObject.transform.position.ToString("f2"));//本地玩家index
                foreach (var item in players)
                {
                    tempPlayerData.Add(item.Key,JsonConvert.SerializeObject(item.Value.GetParm<object>(nameof(PlayerEventName.save))));
                }
                FileSaver.SetMapPlayerCharDatas(center.GetParm<string>(nameof(EventNames.ThisSavePath)), JsonConvert.SerializeObject(tempPlayerData, SerializerHelper.setting));
            }
        }
    }

    #region utility

    void SetDefaultSpawnPoint(Vector3 pos)
    {
        defalutSpawnPoint = pos;
    }

    #endregion

    #region 脚本utility
    void SetDefaultSpawnPoint(string[] pos)
    {
        try
        {
            defalutSpawnPoint = JsonUtility.FromJson<Vector3>(pos[0]);
        }
        catch (System.Exception)
        {
            //MovScriptCommandResolver.ExecuteResult = false;
        }
    }

    #endregion


    void onPause(bool p)
    {
        foreach (var item in players)
        {
            item.Value.SendEvent<bool>(nameof(PlayerEventName.pause),p);//向玩家发送pause一次是暂停两次是解除暂停
        }
    }

    Vector3[] getAllPlayerPosi()
    {
        List<Vector3> posis = new List<Vector3>();
        foreach (var item in players)
        {
            posis.Add(item.Value.gameObject.transform.position);
        }
        return posis.ToArray();
    }
    public EventCenter GetPlayerByIndex(long index)
    {
        return players[index];
    }


    void tempGenPlayer()
    {
        //
        GameObject g = Resources.Load<GameObject>(playerPrefabsPath);
        g = GameObject.Instantiate(g, defalutSpawnPoint,Quaternion.identity);
        //g.transform.position = defalutSpawnPoint;
        

        if (tempPlayerData != null && tempPlayerData.Count > 1)
        {
            //本地玩家index在tempplayerdata[-1]
            EventCenter gc = g.GetComponent<EventCenter>();
            gc.Init(localPlayerIndex);
            //生成玩家预制后写入数据
            players.Add(gc.GetComponent<EventCenter>().UUID, gc);
            gc.SendEvent<object>(nameof(PlayerEventName.load),JObject.Parse(tempPlayerData[localPlayerIndex]));
        }
        else
        {
            Debug.Log("重新生成玩家");
            g.GetComponent<EventCenter>().Init(center.GetParm<long>(nameof(EventNames.GetMaxUUID)));
            g.GetComponent<EventCenter>().SendEvent<object>(nameof(PlayerEventName.load), null);
            Dictionary<long, string> newp = new Dictionary<long, string>();
           
            GameObject t = g;
            players.Add(t.GetComponent<EventCenter>().UUID, t.GetComponent<EventCenter>());
            localPlayerIndex = t.GetComponent<EventCenter>().UUID;
            foreach (var item in players)
            {
                newp.Add(item.Key,JsonConvert.SerializeObject(item.Value.GetParm<object>(nameof(PlayerEventName.save))));
            }
            //g.GetComponent<EventCenter>().SendEvent<object>(nameof(PlayerEventName.load), JObject.Parse(newp[localPlayerIndex]));
            FileSaver.SetMapPlayerCharDatas(center.GetParm<string>(nameof(EventNames.ThisSavePath)), JsonConvert.SerializeObject(newp,SerializerHelper.setting));
        }
    }
    /// <summary>
    /// 当前不使用
    /// </summary>
    /// <returns></returns>
    GameObject GenPlayer()
    {
        //读取预设，生成gameobj，初始化，加载入玩家列表，生成玩家数据字典，写到文件中
        //如果是已经有玩家数据文件，则是读取预设，生成gameobj，初始化，对玩家center使用数据load一遍，以load入的key加载入玩家列表，
        //目前无法区分哪个是本地玩家，只有本地玩家是在buildgame时生成
        GameObject g = Resources.Load<GameObject>(playerPrefabsPath);
        g = GameObject.Instantiate(g);
        g.transform.position = defalutSpawnPoint;
        g.GetComponent<EventCenter>().Init(center.GetParm<long>(nameof(EventNames.GetMaxUUID)));

        Dictionary<long, string> newp = new Dictionary<long, string>();
        GameObject t = g;
        players.Add(t.GetComponent<EventCenter>().UUID, t.GetComponent<EventCenter>());
        foreach (var item in players)
        {
            newp.Add(item.Key, item.Value.GetParm<string>("save"));
        }
        FileSaver.SetMapPlayerCharDatas(center.GetParm<string>(nameof(EventNames.ThisSavePath)), JsonConvert.SerializeObject(newp,SerializerHelper.setting));

        return g;
    }
}
