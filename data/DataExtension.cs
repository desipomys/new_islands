using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public static class DataExtension
{
    public static int[] GetCoord3(this long xyh)
    {//0-23位是x,24-47是y,48-63是h
        //x:0-65536*256,y:65536*256,h:65536
        ulong temp = (ulong)xyh;
        int[] ans = new int[3];
        ans[0] = (int)((temp >> 40) - 65536*128);
        ans[1] = (int)(((xyh & 0x000000ffffff0000) >> 16) - 65536 * 128);
        ans[2] = (int)((xyh & 0x000000000000ffff) - 32768);
        return ans;
    }
    public static int[] GetCoord3(this int xyz)//
    {///xxxxxxxx|zzzzzzzz|yyyyyyyyyyyyyyyy
        int[] ans = new int[3];
        ans[0] = (byte)(xyz >> 24)-128;
        ans[1] = ((xyz & 0x00ffffff)>>16)-128;
        ans[2] = (xyz & 0x0000ffff)-32768;
        return ans;
    }

    public static int AddCoord3X(this int xyz,int x)
    {
        int[] ans = xyz.GetCoord3();
        return XYHelper.ToCoord3(ans[0]+x,ans[1],ans[2]);
    }
    public static int AddCoord3H(this int xyz, int x)
    {
        int[] ans = xyz.GetCoord3();
        return XYHelper.ToCoord3(ans[0] , ans[1], ans[2]+ x);
    }
    public static int AddCoord3Y(this int xyz, int x)
    {
        int[] ans = xyz.GetCoord3();
        return XYHelper.ToCoord3(ans[0] , ans[1]+ x, ans[2]);
    }

    public static int ToInt(this Vector3Int v)//
    {
        int i;
        i = XYHelper.ToCoord3(v.x, v.z, v.y);
        //i = ((v.x+128)&0x000000ff) << 24 | ((v.z+128) & 0x000000ff) << 16 | ((v.y+32768) & 0x0000ffff);
        return i;
    }
    

    public static int GetX(this long xy)
    {
        return (int)(xy >> 32);
    }
    public static int GetY(this long xy)
    {
        return (int)(xy % 0x0000000100000000);
    }

    public static int GetX(this int xy)
    {
        //short s= (short)(xy / 0x00010000);
        return (int)(xy>>16);
    }
    public static int GetY(this int xy)
    {
        short s= (short)(xy % 0x00010000);
        return (int)s;
    }
    public static Vector3 FromString(this Vector3 vec3, string data)
    {
        data= data.Replace("(", " ").Replace(")", " ");
        string[] temp=data.Split(',');
        if(temp.Length!=3)return Vector3.zero;
        
        try
        {
             vec3.x=float.Parse(temp[0]);
            vec3.y=float.Parse(temp[1]);
            vec3.z=float.Parse(temp[2]);
        
        }
        catch (System.Exception)
        {
            return Vector3.zero;
        }
       return vec3;
    }
    public static Vector3 GetPos(this Matrix4x4 mat)
    {
        Vector3 v;
        v.x = mat.m03;
        v.y = mat.m13;
        v.z = mat.m23;
        return v;
    }
    public static Vector3 GetSize(this Matrix4x4 mat)
    {
        Vector3 s;
        s.x = new Vector4(mat.m00,mat.m10,mat.m20,mat.m30).magnitude;
        s.y = new Vector4(mat.m01, mat.m11, mat.m21, mat.m31).magnitude;
        s.z = new Vector4(mat.m02, mat.m12, mat.m22, mat.m32).magnitude;
        return s;
    }
    public static Quaternion GetQuaternion(this Matrix4x4 mat)
    {
        return Quaternion.LookRotation(mat.GetColumn(2), mat.GetColumn(1));
    }

    public static DIR[] UnPack(this DIR dir)
    {
        DIR[] ds = new DIR[3];
        if(dir.Contain(DIR.front))
        {
            if (dir.Contain(DIR.back))
            {
                ds[0] = DIR.none;
            }
            else
            {
                ds[0] = DIR.front;
            }
        }
        else if(dir.Contain(DIR.back))
        {
            ds[0] = DIR.back;
        }

        if (dir.Contain(DIR.up))
        {
            if (dir.Contain(DIR.down))
            {
                ds[1] = DIR.none;
            }
            else
            {
                ds[1] = DIR.up;
            }
        }
        else if (dir.Contain(DIR.down))
        {
            ds[1] = DIR.down;
        }

        if (dir.Contain(DIR.left))
        {
            if (dir.Contain(DIR.right))
            {
                ds[2] = DIR.none;
            }
            else
            {
                ds[2] = DIR.left;
            }
        }
        else if (dir.Contain(DIR.right))
        {
            ds[2] = DIR.right;
        }

        return ds;
    }
    public static Vector3 GetAngle(this DIR dir)
    {//front/back=x的+-,up/down=y的+-,left/right=z的+-
        Vector3 ans=new Vector3();
        if(dir.Contain(DIR.front))ans.x+=1;
        if(dir.Contain(DIR.back))ans.x-=1;

        if(dir.Contain(DIR.up))ans.y+=1;
        if(dir.Contain(DIR.down))ans.y-=1;

        if(dir.Contain(DIR.left))ans.z+=1;
        if(dir.Contain(DIR.right))ans.z-=1;

        
        return Quaternion.LookRotation(ans,Vector3.up).eulerAngles;
    }

    public static bool Contain(this DIR kind, DIR d)
    {
        return (kind & d) != DIR.none;
    }
    public static bool Contain(this KeyCodeKind kind,KeyCodeKind k)
    {
        return (kind & k) != KeyCodeKind.None;
    }
    public static bool Contain(this Movement_Stat stat,Movement_Stat k)
    {
        return (stat & k) != Movement_Stat.none;
    }
    public static bool Contain(this NPCDataChgPOS stat, NPCDataChgPOS k)
    {
        return (stat & k) != NPCDataChgPOS.none;
    }
    public static bool Contain(this ItemCompareMode stat,ItemCompareMode k)
    {
        return (stat & k) != ItemCompareMode.none;
    }

    public static bool IsRunning(this Movement_Stat stat)
    {
        if ((stat & Movement_Stat.walk) != Movement_Stat.none && (stat & Movement_Stat.run) != Movement_Stat.none) return true;
        return false;
    }
    public static Movement_Stat Remove(this Movement_Stat stat, Movement_Stat target)
    {
        return stat & (~target);
    }
    public static Movement_Stat Add(this Movement_Stat stat, Movement_Stat target)
    {
        return stat | target;
    }

    public static bool Contain(this EntityType et,EntityType typ)
    {
        return (et & typ) != EntityType.modern;
    }
    public static EntityType GetSubType(this EntityType et)
    {
        return et & (~EntityType.fantacy);
    }

    public static bool IsLiqud(this B_Material bm)
    {
        return (int)bm >= 200;
    }
    public static int Level(this B_Material bm)
    {
        if ((int)bm>200)
        {
            return 200;
        }
        else if((int)bm > 180)
        {
            return 3;
        }
        else if ((int)bm > 150) { return 2; }
        else if ((int)bm > 100) { return 1; }
        else if ((int)bm > 50) { return 0; }
        else return -1;
    }

    #region itempageExtension
    public static void ClearItems(this Item[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = Item.Empty;
        }
    }
    public static void SetSize(this Item[] items,int x, int y)
    {
        items = new Item[x];
    }

    public static void SetItems(this Item[] items,Item[] its, int[,] placements)
    {
        items = its;
    }

    public static Item[] GetItems(this Item[] items)
    {
        return items;
    }
    public static Item[,] GetBigItems(this Item[,] items)
    {
        return items;
    }

    //public static int[,] GetPlacements(this Item[] items) {}

    #region  set
    public static bool SetItemAt(this Item[] items,Item i, int x, int y)
    {
        try
        {
            items[x] = i;
            return true;
        }
        catch (System.Exception)
        {

            return false;
        }
        

    }
    public static bool DeleteItemAt(this Item[] items, int x, int y)
    {
        try
        {
            items[x] = Item.Empty ;
            return true;
        }
        catch (System.Exception)
        {

            return false;
        }
    }

    //public static int AddItem(this Item[] items, Item i){}
    public static int AddItemAt(this Item[] items, Item i, int x, int y)
    {
        try
        {
    if (!Item.IsNullOrEmpty(items[x]))
            {
                if (items[x].Equals(i))//二者相等
                {
                    int p1 = items[x].SafeAdd(i.num, i.level);
                
                    return p1;
                }
                else return -1;//不相等
            }
            else
            {
                items[x] = i;
            
                return 0;
            }
        }
        catch (System.Exception)
        {

            return -1;
        }
        
    }
    public static int AddItemNumAt(this Item[] items, int i, int x, int y)
    {
        try
        {
if (!Item.IsNullOrEmpty(items[x]))
        {
            int p1 = items[x].SafeAdd(i, -1);
            
            return p1;
        }
        else return -1;
        }
        catch (System.Exception)
        {
            return -1;
        }
        
    }
    
    public static int SubItemAt(this Item[] items, Item item, int x, int y, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        try
        {
            if (Item.IsNullOrEmpty(item)) return 0;
            if (!item.Compare(items[x], mode)) return -1;
            if (item.num > items[x].num)
            {
                items[x].num = 0;
                
                return 0;
            }
            else
            {
                items[x].num -= item.num;
                
                return items[x].num;
            }
        }
        catch (System.Exception)
        {
            return -1;
        }
    }
    public static int SetItemNumAt(this Item[] items, int num, int x, int y)
    {
        try
        {
            if (!Item.IsNullOrEmpty(items[x]))
            {
                int p= items[x].SafeSet(num);
                return p;
            }
            else return -1;
        }
        catch (System.Exception)
        {
            return -1;
        }
    }

    #endregion

    #region  get
    public static int Contain(this Item[] items, Item item, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        int count = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (item.Compare(items[i], mode))
            {
                count += items[i].num;
            }
        }
        return count;
    }
    public static int ContainAt(this Item[] items, Item item, int x, int y, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        try
        {
            if (items[x].Compare(item, mode))
            {
                return items[x].num;
            }
            else return -1;
        }
        catch (System.Exception)
        {
            return 1;
        }
    }
    public static Item GetItemAt(this Item[] items, int x, int y)
    {
        try
        {
             return items[x];
        }
        catch (System.Exception)
        {
            return null;
        }
       
    }
    public static int CountItem(this Item[] items, Item i, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return items.Contain(i, mode);
    }
    public static bool CanPlaceAt(this Item[] items, Item item, int x, int y, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        try
        {
            if (item.Compare(items[x])) return true;
            else if (Item.IsNullOrEmpty(items[x])) return true;
            else return false;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
    //public static int[] GetItemLeftUp(this Item[] items, int x, int y){}
    //public static bool IsBigChest(this Item[] items) {}
    #endregion
    #endregion

    #region unityExtension

    public static void SetText(this Button but,string txt)
    {
        but.GetComponentInChildren<Text>().text = txt;
    }

    #endregion

    public static BuffValueModifyTarget_base GetInstance(this BuffValueModifyTarget_Type s)
    { //Debug.Log(s);
        return BuffValueModifyTargetFactory.GetInstance(s);
    }
    /// <summary>
    /// 利用反射实现深复制，可递归，尽量少用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T DeepCopy<T>(T obj)
    {

        //如果是字符串或值类型则直接返回

        if (obj is string || obj.GetType().IsValueType) return obj;

        object retval = System.Activator.CreateInstance(obj.GetType());

        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        foreach (FieldInfo field in fields)

        {

            try { field.SetValue(retval, DeepCopy(field.GetValue(obj))); }

            catch { }

        }

        return (T)retval;

    }
    /// <summary>
    /// 利用反射实现深复制，可递归，尽量少用
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static object DeepCopy(object obj)
    {
        if (obj is string || obj.GetType().IsValueType) return obj;

        object retval = System.Activator.CreateInstance(obj.GetType());

        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        foreach (FieldInfo field in fields)

        {

            try { field.SetValue(retval, DeepCopy(field.GetValue(obj))); }

            catch { }

        }

        return retval;
    }

}
