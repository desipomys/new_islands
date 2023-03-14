using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;

public enum ItemGroup
{
    hide=-1,
    resource,//资源,工具,武器,枪械,装备,配件,食物,皮肤,其他,可放置
    tool,
    weapon,
    gun,
    equip,
    part,
    food,
    skin,
    other,
    block
}


public enum ItemCompareMode
{
    absEqual = 31,//完全相同
    excludeNum = 27,
    excludeNumExd = 11,

    none=0,
    idEqual = 1,
    subidEqual = 2,
    numEqual = 4,
    levelEqual = 8,
    exdEqual = 16
}
/// <summary>
/// item的EXD携带的内容中回调点枚举
/// </summary>
public enum ItemContent
{/// <summary>
/// 当物品在UI中被左键点击时，早于onUIexchange
/// </summary>
    OnUILeftHit,
    /// <summary>
    /// 当物品在UI中被右键点击时，早于onUIexchange
    /// </summary>
    OnUIRightHit,
    /// <summary>
    /// 当物品被丢弃时
    /// </summary>
    OnDorp,
    /// <summary>
    /// 当物品与持有item的鼠标手交换时
    /// </summary>
    OnUIExchange,
    /// <summary>
    /// 当物品被捡起时
    /// </summary>
    OnPick,
    /// <summary>
    /// 当物品被创造的时间到达时，需要保存已持有时间和接收update方法以更新时间，不容易实现
    /// </summary>
    OnTickEnd,
    /// <summary>
    /// tag数组
    /// </summary>
    Tag,
    /// <summary>
    /// 弹药轮转数组:item[]
    /// </summary>
    AmmoItemList,
    /// <summary>
    /// 当前弹药index:int
    /// </summary>
    CurrentAmmoIndex,
}

[Serializable]
public class Item: IScriptDataGetter
{
    public static readonly Item Empty = new Item();
    /// <summary>
    /// 当物品在箱子中的最大数量的倍数
    /// </summary>
    public static readonly int ChestMaxNumFacter = 10;
    public static readonly int LevelFactor;
    public int id, subid, num, level;

    [JsonIgnore]
    public int GetTrueLevel { get { return level / LevelFactor; } set { } }
    //level 0-1024代表等级0，1024-2048代表等级1，2048-3072代表等级2，合成、背包转移等行为会使level混合
    public bool rota=false;
    /// <summary>
    /// exd存放string,str[]对，str是事件触发回调点，str[]是发生时执行的movscript逻辑
    /// </summary>
    [DictionaryDrawerSettings()]
    [ShowInInspector]
    public Dictionary<ItemContent, object> exd;//exd应该分类型，否则将给序列化/反序列化造成麻烦

    [JsonConverter(typeof(DatetimeConverter))]
    public DateTime createTime;

    int[] sizeCache;


    public Item() { }
    public Item(int id) { this.id = id; }
    public Item(int id, int num) { this.id = id; this.num = num; }
    public Item(int id, int subid,int num) { this.id = id;this.subid=subid; this.num = num; }
    public Item(Item_Warper iw)
    {
        CopyFrom(iw.item);
    }
    public Item(Item it)
    {
        id = it.id;
        subid = it.subid;
        num = it.num;
        level = it.level;
        exd = it.exd;
    }

    #region getset
    public int IsNumOverFlow()
    {
      
        int maxNum = EventCenter.WorldCenter.GetParm<int, int>("ItemIDtoMax", id);
        return Mathf.Clamp(num - maxNum, 0, num);
    }
    public int IsNumOverFlow(bool isbig)
    {
        int factor = isbig ? ChestMaxNumFacter : 1;
        int maxNum = EventCenter.WorldCenter.GetParm<int, int>("ItemIDtoMax", id)*factor;
        return Mathf.Clamp(num - maxNum, 0, num);
    }
    public static bool IsNullOrEmpty(Item i)
    {
        if (i == null) return true;
        if (i.id == 0 || i.num <= 0) return true;
        return false;
    }
    public bool Compare(Item i, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        bool a = Item.IsNullOrEmpty(i);
        bool b = Item.IsNullOrEmpty(this);
        if (a ^ b) return false;
        if (a) return true;//二者皆空
        //bool cache=true;
        if ((mode & ItemCompareMode.idEqual) != 0)//idequal位为1
        {
            if (i.id != id) { return false; }//不等则返回，等则下一步
        }
        if ((mode & ItemCompareMode.subidEqual) != 0)//subidequal位为1
        {
            if (i.subid != subid) { return false; }//不等则返回，等则下一步
        }
        if ((mode & ItemCompareMode.numEqual) != 0)//numequal位为1
        {
            if (i.num != num) { return false; }//不等则返回，等则下一步
        }
        /*if ((mode & ItemCompareMode.levelEqual) != 0)//levelequal位为1
        {
            if (i.level != level) { return false; }//不等则返回，等则下一步
        }*/
        if ((mode & ItemCompareMode.exdEqual) != 0)//exdequal位为1
        {
            if (i.exd != exd) { return false; }//不等则返回，等则下一步
        }
        return true;//全部比较都通过返回真
    }
    /// <summary>
    /// 返回num设置完剩下多少
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public int SafeSet(int num)
    {
        int maxNum = EventCenter.WorldCenter.GetParm<int, int>("ItemIDtoMax", id);
        this.num = Mathf.Clamp(num, 0, maxNum);
        return Mathf.Clamp(num - this.num, 0, num);
    }
     /// <summary>
    /// 返回num设置完剩下多少
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public int SafeSet(int num,bool isbig)
    {
        int factor=isbig?ChestMaxNumFacter:1;
        int maxNum = EventCenter.WorldCenter.GetParm<int, int>("ItemIDtoMax", id)*factor;
        this.num = Mathf.Clamp(num, 0, maxNum);
        return Mathf.Clamp(num - this.num, 0, num);
    }
    /// <summary>
    /// 返回不够减的数量
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public int SafeSub(int num)
    {
        if (num < 0) return 0;
        int temp = Mathf.Clamp(num - this.num, 0, num);
        this.num = Mathf.Clamp(this.num - num, 0, this.num);
        return temp;
    }
    public int SafeAdd(Item i)
    {
       return SafeAdd(i.num, i.level);
    }
    /// <summary>
    /// level=-1代表level不变
    /// </summary>
    /// <param name="num"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public int SafeAdd(int num,int level)
    {
        if (level == -1) level = this.level;
        int oldnumLevel = this.num * this.level;
        int oldnum = this.num;
        int maxNum = EventCenter.WorldCenter.GetParm<int, int>("ItemIDtoMax", id);
        int sum = this.num + num;
        this.num = Mathf.Clamp(sum, 0, maxNum);
        this.level = (oldnumLevel  + (this.num - oldnum) * level) / this.num;//level混合
        
        return Mathf.Clamp(sum - this.num, 0, sum);
    }
     /// <summary>
    /// level=-1代表level不变
    /// </summary>
    /// <param name="num"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public int SafeAdd(int num,int level,bool isbig)
    {
        if (level == -1) level = this.level;
        int oldnumLevel = this.num * this.level;
        int oldnum = this.num;
        int factor=isbig?ChestMaxNumFacter:1;
        int maxNum = EventCenter.WorldCenter.GetParm<int, int>("ItemIDtoMax", id)*factor;
        int sum = this.num + num;
        this.num = Mathf.Clamp(sum, 0, maxNum);
        this.level = (oldnumLevel  + (this.num - oldnum) * level) / this.num;//level混合
        
        return Mathf.Clamp(sum - this.num, 0, sum);
    }
    /// <summary>
    /// 返回加完后的输入余下多少
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public int TrySafeAdd(int num)
    {
        int maxNum = EventCenter.WorldCenter.GetParm<int, int>("ItemIDtoMax", id);
        int sum = this.num + num;
        int trynum = Mathf.Clamp(sum, 0, maxNum);
        return Mathf.Clamp(sum - trynum, 0, sum);
    }
    /// <summary>
    /// 返回加完后的输入余下多少
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public int TrySafeAdd(int num,bool isbig)
    {
        int factor=isbig?ChestMaxNumFacter:1;
        int maxNum = EventCenter.WorldCenter.GetParm<int, int>("ItemIDtoMax", id)*factor;
        int sum = this.num + num;
        int trynum = Mathf.Clamp(sum, 0, maxNum);
        return Mathf.Clamp(sum - trynum, 0, sum);
    }
    /// <summary>
    /// 0 width;1 height
    /// </summary>
    /// <returns></returns>
    public int[] GetSize()
    {
        if(sizeCache==null||sizeCache.Length==0)sizeCache=XYHelper.GetIntXY(EventCenter.WorldCenter.GetParm<int, int>(nameof(EventNames.ItemIDtoSize), id));
        if(rota){return new int[]{sizeCache[1],sizeCache[0]};}
        return sizeCache;
    }
    /// <summary>
    /// 0 width;1 height
    /// </summary>
    /// <returns></returns>
    public int[] GetOriginSize()
    {
        if (sizeCache == null || sizeCache.Length == 0) sizeCache = XYHelper.GetIntXY(EventCenter.WorldCenter.GetParm<int, int>(nameof(EventNames.ItemIDtoSize), id));
        //if (rota) { return new int[] { sizeCache[1], sizeCache[0] }; }
        return sizeCache;
    }
    public string GetDescript()
    {
        return EventCenter.WorldCenter.GetParm<int, string>(nameof(EventNames.ItemIDtoDes), id);
    }
    public string GetItemName()
    {
        return EventCenter.WorldCenter.GetParm<int, string>(nameof(EventNames.ItemIDtoName), id);
    }
    public int GetMaxSub()
    {
        return EventCenter.WorldCenter.GetParm<int, int>(nameof(EventNames.ItemIDtoMaxSub), id);
    }
    public int GetMaxNum()
    {
        return EventCenter.WorldCenter.GetParm<int, int>(nameof(EventNames.ItemIDtoMax), id);
    }
    #endregion

    #region exdOperate
    //exd里可以存放content和var两种内容，
    //content是事件回调点+执行内容组成的对
    //var是名称+变量值组成的对，var支持int,float,str,item,damage,buff
    public bool IsExdEmpty()
    {
        if (exd == null) return true;
        if (exd.Count == 0) return true;
        return false;
    }
    public bool ContainContent(ItemContent content)
    {
        if (exd!=null &&exd.ContainsKey(content))
        {
            if (exd[content] != null) return true;
            return false;
        }
        return false;
    }
    public void SetContent(ItemContent content, string[] c)
    {
        if (exd != null && exd.ContainsKey(content))
        {
            exd[content] = c;
        }
        else
        {
            if (exd == null) exd = new Dictionary<ItemContent, object>();
            exd.Add(content, c);
        }
    }
    public object GetContent(ItemContent content)
    {
        if (exd != null && exd.ContainsKey(content))
        {
            try
            {
                return exd[content];
            }
            catch (Exception)
            {
                Debug.Log(content + "不是str[]类型");
                throw;
            }

        }
        else return null;
    }

    public void SetVar<T>(string name,T value)
    {
        ItemContent cont = (ItemContent)Enum.Parse(typeof(ItemContent), name);
        if (exd != null && exd.ContainsKey(cont))
        {
            exd[cont] = value;
        }
        else
        {
            if (exd == null) exd = new Dictionary<ItemContent, object>();
            exd.Add(cont, value);
        }
    }
    public T GetVar<T>(string name)
    {
        ItemContent cont = (ItemContent)Enum.Parse(typeof(ItemContent), name);
        //name =typeof(T).Name+"-"+name;
        if (exd != null && exd.ContainsKey(cont))
        {
           return (T)exd[cont];
        }
        else return default(T);
    }
    #endregion

    #region ScriptGetSet
    public object GetScriptData(int target,int parm)
    {
        switch (target)
        {
            case 0:return id;break;
            case 1:return subid;break;
            case 2:return num;break;
            case 3:return level;break;
            case 4:return rota;break;
            case 5:return exd;break;
            case 6:return createTime;break;
            default:return num;break;
        }
    }
    public void SetScriptData(int target,int parm,object o)
    {
        switch (target)
        {
            case 0:id=(int)o;break;
            case 1:subid=(int)o;break;
            case 2:num=(int)o;break;
            case 3:level=(int)o;break;
            case 4:rota=(bool)o;break;
            case 5:exd=(Dictionary<ItemContent,object>)o;break;
            case 6:createTime=(DateTime)o;break;
            default:break;
        }
    }
    #endregion

    public virtual void CopyFrom(Item i)
    {
        id = i.id;
        subid = i.subid;
        num = i.num;
        level = i.level;
        exd = i.exd;
        rota = i.rota;
    }
    public void CopyFrom(Item i,ItemCompareMode mod)
    {
        if(mod.Contain(ItemCompareMode.idEqual))
        id = i.id;
        if (mod.Contain(ItemCompareMode.subidEqual))
            subid = i.subid;
        if (mod.Contain(ItemCompareMode.numEqual))
            num = i.num;
        if (mod.Contain(ItemCompareMode.levelEqual))
            level = i.level;
        if (mod.Contain(ItemCompareMode.exdEqual))
            exd = i.exd;
        rota = i.rota;
    }

    public override string ToString()
    {
        //DataWarper dw = new DataWarper(this);
        return JsonConvert.SerializeObject(this, typeof(Item), SerializerHelper.setting);
    }

    public static Item FromString(string data)
    {
        //DataWarper dw = DataWarper.FromString(data);
        return JsonConvert.DeserializeObject<Item>(data, JsonSetting.serializerSettings);
    }
}

//专供本地存储使用
/*public class Item_Warper
{
    public Item item;
    public int max;
    public int maxsub;
    public int x;//在UI的大小
    public int y;

    public string texture;
    public string descript;

    public Dictionary<string,string> exdType;
}*/
