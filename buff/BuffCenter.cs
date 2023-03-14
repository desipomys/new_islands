using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

/// <summary>
/// 未测试，谨慎使用
/// </summary>
public class BuffCenter: MonoBehaviour,IEventRegister
{//装备+防御也靠buff实现
    //key是buff的名字属性,每个list只取最高级level的执行其update
    EventCenter center;
    public Dictionary<string,BaseBuff> allBuff=new Dictionary<string,BaseBuff>(); 
    TwoKeyDictionary<string, BuffValueModifyTarget_base, BuffValueModifyData_base> valueModifyList = new TwoKeyDictionary<string, BuffValueModifyTarget_base, BuffValueModifyData_base>();
    /// <summary>
    /// 保存该变量的原始值
    /// </summary>
    Dictionary<BuffValueModifyTarget_base, FP> originValue = new Dictionary<BuffValueModifyTarget_base, FP>();

    public void OnDestroy()
    {
        
        //还原数值为存储固定值
    }

    #region  改值功能
    //只需int,float,str三种类型
    //想改值需提交 buff名，改值目标，改值数据
    //<buffname,buffvaluemodifier,ValueModifyUnit,>
   public void ModifyOriginValue(string buffname,BuffValueModifyTarget_base target,BuffValueModifyData_base data)
   {
        target = target.GetInstance();
        if (!originValue.ContainsKey(target))//此值之前没改过
        {
            FP temp=target.valueGetter(center);
            temp=data.Apply(temp);
            target.valueSetter(temp,center);
        }
        else
        {
            FP temp=originValue[target];
            temp=data.Apply(temp);
            originValue[target]=temp;
            FlushTargetValue(target);
        }
   }

    public void ModifyValue(string buffname,BuffValueModifyTarget_base target,BuffValueModifyData_base data)
    {
        //target =target.GetInstance();
        if (originValue.ContainsKey(target))
            Debug.Log("mod前"+originValue[target]);
        if(!originValue.ContainsKey(target))//此值之前没改过
        {
            if(valueModifyList.ContainsKey(buffname))//此buff之前提交过改值申请
            {
                
                //add valueModifyList
                valueModifyList.Add(buffname, target, data);
                //获取原始值add originValue
                FP tem = target.valueGetter(center);
                if(!originValue.ContainsKey(target))//此值之前没被改过
                    originValue.Add(target, tem);

                //应用修改到原始值上
                FlushTargetValue(target);
            }
            else
            {
                //add valueModifyList
                valueModifyList.Add(buffname, target, data);
                //获取原始值
                //获取原始值add originValue
                FP tem = target.valueGetter(center);
                if (!originValue.ContainsKey(target))//此值之前没被改过
                    originValue.Add(target, tem);
                //应用修改到原始值上
                FlushTargetValue(target);
            }
        }
        else//此值之前改过
        {
            if (valueModifyList.ContainsKey(buffname))//此buff之前提交过改值申请
            {
                //add valueModifyList
                valueModifyList.Add(buffname, target, data);
                FlushTargetValue(target);
            }
            else
            {
                //add valueModifyList
                valueModifyList.Add(buffname, target, data);
                
                FlushTargetValue(target);
            }
        }
        if (originValue.ContainsKey(target))
            Debug.Log("mod后"+originValue[target]);
    }

    public void CancelModifyValue(string buffname, BuffValueModifyTarget_base target, BuffValueModifyData_base data)
    {
        target = target.GetInstance();
        if (!originValue.ContainsKey(target))//此值之前没改过
        {
            
        }
        else//此值之前改过
        {
            if (valueModifyList.ContainsKey(buffname))//此buff之前提交过改值申请
            {
                //add valueModifyList
                valueModifyList[buffname].Remove( target);
                FlushTargetValue(target);
            }
            else//本buff尝试取消其他buff添加的改值申请
            {
                
            }
        }
    }
    //将buff修改值应用到原始值上
    public void FlushTargetValue(BuffValueModifyTarget_base target)
    {
        List<BuffValueModifyData_base> datas = valueModifyList.GetValuesByK2(target);
        FP temp = new FP(originValue[target]);
        //Debug.Log("原始值为" + temp);
        for (int i = 0; i < datas.Count; i++)//生成一个对原始值应用了多次修改后的fp
        {//是否正确存疑
            //Debug.Log(datas[i].Apply(originValue[target]).data+ target.valueType().Name);
            temp.Add(datas[i].Apply(originValue[target]), target.valueType());
        }
        target.valueSetter(temp, center);

        //Debug.Log("改后值为" + target.valueGetter(center).data);
    }
    #endregion

    public void AddBuff(EventCenter source,BaseBuff buff)
    {
        bool succ = true;
            foreach (var item in allBuff)
            {
                succ = succ | item.Value.BeforeBuffAdd(source, center, buff);
            }
            if (!succ) return;
        if(allBuff.ContainsKey(buff.buffname))
        {
            allBuff[buff.buffname].OnStackLevel(source,center,buff);
        }
        else
        {
            allBuff.Add(buff.buffname, buff);
            buff.OnAttach(source, center);
        }
        foreach (var item in allBuff)
        {
            item.Value.AfterBuffadd(source, center, buff);
        }
    }
    public void RemoveBuff(EventCenter source, BaseBuff buff)
    {
        if (!allBuff.ContainsKey(buff.buffname))
        {return;
            
        }
        
        bool succ = true;
        foreach (var item in allBuff)
        {
            succ = succ | item.Value.BeforeBuffDestory(source, center, buff);
        }
        if (!succ) return;
        allBuff[buff.buffname].OnDestory(source, center);

        if (valueModifyList.ContainsKey(buff.buffname))
        {
            List<BuffValueModifyTarget_base> valueToRecover = new List<BuffValueModifyTarget_base>(valueModifyList[buff.buffname].Keys);
            valueModifyList.Remove(buff.buffname);
            for (int i = 0; i < valueToRecover.Count; i++)
            {
                FlushTargetValue(valueToRecover[i]);
            }
        }
        
        foreach (var item in allBuff)
        {
            item.Value.AfterBuffDestory(source, center, buff);
        }

    }
    public void Update()
    {
        foreach (var item in allBuff)
        {
            item.Value.OnUpdate(center);
        }
    }
 public void OnEventRegist(EventCenter e)
    {
        center = e;
        center.RegistFunc<BuffCenter>("buffCenter",()=> { return this; });
    }
    public void FromObject(object data)
    {
        //反序列化完成后

        //调用所有buff的init
        foreach (var item in allBuff)
        {
            item.Value.OnBuffInit(center,this);
        }

    }
    public object ToObject()
    {

        return null;
    }

    public void AfterEventRegist()
    {
        //throw new NotImplementedException();
    }
}



public enum buffModifyMode
{
    add,percent,mul,set
}
public abstract class BuffValueModifyData_base
{
    public BaseBuff buff;
    /// <summary>
    /// true +模式  false %模式
    /// </summary>
    public buffModifyMode mode;
    public abstract FP Apply(FP fp);

    /// <summary>
    /// 不会修改原始值
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public FP CalcAdd(FP origin,FP value)
    {
        //origin只有一个字段有意义，value一般也只有一个有意义
        //设1=int,2=float,3=str，value同理
        //1 1 =int,int型运算
        //1 2 =int,float型运算
        //只支持int,float混合运算，str不能参与add/percent运算
        return new FP(value);
    }

    /// <summary>
    /// 按百分比加值
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public FP CalcPercent(FP origin, FP value)
    {
        //origin只有一个字段有意义，value一般也只有一个有意义
        //设1=int,2=float,3=str，value同理
        //1 1 =int,int型运算
        //1 2 =int,float型运算
        //只支持int,float混合运算，str不能参与add/percent运算
        float ans = origin;

        ans *= (value/100);

        return ans;
    }

    public FP CalcMul(FP origin, FP value)
    {
        //origin只有一个字段有意义，value一般也只有一个有意义
        //设1=int,2=float,3=str，value同理
        //1 1 =int,int型运算
        //1 2 =int,float型运算
        //只支持int,float混合运算，str不能参与add/percent运算
        float ans = origin;

        ans *= (value);

        return ans;
    }

    public FP CalcSet(FP origin, FP value)
    {
        return new FP(value);
    }

}
/// <summary>
/// buff改值静态数据
/// </summary>
/// <typeparam name="T"></typeparam>
public class BuffStaticValueModifyData:BuffValueModifyData_base
{
    public FP value;
    /// <summary>
    /// true +模式  false %模式
    /// </summary>

    public BuffStaticValueModifyData(BaseBuff b, FP v, buffModifyMode mode)
    {
        buff = b;
        value = v;
        this.mode = mode;
    }
    /// <summary>
    /// 返回被修改过后的原值
    /// </summary>
    /// <param name="fp">从具体数据生成出的原值，只有一个字段有意义</param>
    /// <returns></returns>
    public override FP Apply(FP fp)
    {
        FP nfp = null;
        switch (mode)
        {
            case buffModifyMode.add:
                nfp = base.CalcAdd(fp,value);
                break;
            case buffModifyMode.percent:
                nfp = base.CalcPercent(fp, value);
                break;
            case buffModifyMode.set:
                nfp = base.CalcSet(fp, value);
                //nfp.Fixed = true;
                break;
            case buffModifyMode.mul:
                nfp = base.CalcMul(fp, value);
                break;
            default:
                break;
        }
        return nfp;
    }
}
/// <summary>
/// buff改值动态数据
/// </summary>
/// <typeparam name="T"></typeparam>
public class BuffDynamicValueModifyData: BuffValueModifyData_base
{
    public Func<FP> value;
    /// <summary>
    /// true +模式  false %模式
    /// </summary>

    public BuffDynamicValueModifyData(BaseBuff b, Func<FP> v, buffModifyMode mode)
    {
        buff = b;
        value = v;
        this.mode = mode;
    }
    public override FP Apply(FP fp)
    {
        FP nfp=null;
        switch (mode)
        {
            case buffModifyMode.add:
                nfp = base.CalcAdd(fp, value());
                break;
            case buffModifyMode.percent:
                nfp = base.CalcPercent(fp, value());
                break;
            case buffModifyMode.set:
                nfp = base.CalcSet(fp, value());
                //nfp.Fixed = true;
                break;
            case buffModifyMode.mul:
                nfp = base.CalcMul(fp, value());
                break;
            default:
                break;
        }
        return nfp;
    }
}