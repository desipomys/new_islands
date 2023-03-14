using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchGraph :Base_TechData
{
    public string[] condition;
    public string Detail_Descript;
    /// <summary>
    /// 解锁所需资源
    /// </summary>
    public Resource_Data unlockRes;
    /// <summary>
    /// 解锁所需item
    /// </summary>
    public List<Item> unlockItems;
}
/// <summary>
/// 玩家的研究队列数据
/// </summary>
public class ResearchData
{//研究队列+已解锁
    public List<int> unlocked=new List<int>();
    public List<int> started = new List<int>();
    public List<float> progress = new List<float>();

    public bool ContainUnlock(int p)
    {
        return unlocked.Contains(p);
    }
    public bool ContainStart(int p)
    {
        return started.Contains(p);
    }
}