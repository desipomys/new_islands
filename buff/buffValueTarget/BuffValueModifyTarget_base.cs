using System;

public interface ValueModifyTarget_base
{
     void valueSetter(FP p, EventCenter center);
     FP valueGetter(EventCenter center);
     ValueModifyTarget_base getInstance();
     Type valueType();
}
/// <summary>
/// buff改值目标，getter,setter指向具体组件上的值
/// </summary>
public abstract class BuffValueModifyTarget_base:ValueModifyTarget_base
{
    //public abstract Type ValueType { private set;get return valueType() }
    public abstract void valueSetter(FP p, EventCenter center);
    public abstract FP valueGetter(EventCenter center);
    public ValueModifyTarget_base getInstance(){return null;}
    public abstract BuffValueModifyTarget_base GetInstance();
    public abstract Type valueType();
}

