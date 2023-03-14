using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// ֻ��������Ҫ��ҩ�ķ����ӵ��ģ��������³���
/// </summary>
public class ToolEffector_FireBullet : ToolEffector
{
    /// 
    ///���ṩ�� �ӵ�λ��*�̶�����enum
    ///   �ٶ�*excel+buff
    ///   ���*excel+buff
    ///   ����ʱ��
    ///   �ӵ�����*enum��excel+buff
    ///   �˺���Ԫ*prefabs,��������excel+buff
    ///   ���ֵ�Ԫ
    ///   ��Դ��tool-ʵʱֵ
    ///   ������uuid-ʵʱֵ
    /// 
    // [LabelText("����λ��")]
    // public ValueSource_base bulletPos;
    // [LabelText("�ӵ��ٶ�")]
    // public ValueSource_base speed;
    // [LabelText("���")]
    // public ValueSource_base range;
    // [LabelText("����ʱ��")]
    // public ValueSource_base time;
    // [LabelText("�ӵ�����")]
    // public ValueSource_base type;
    // [LabelText("�˺�����")]//����fp[],ֻ��Ӧ�˺�ֵ������
    // public ValueSource_base damageParm;

    /// <summary>
    /// һ���Ǹ���ȡbulletexcel��Ч����
    /// </summary>
    public ValueSource_base bulletValue;


    //public float Penatra;
    ToolCurrentAmmo currentAmmo = new ToolCurrentAmmo();

    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

   
    BulletParm parm = new BulletParm();
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        ToolEngine eng=(ToolEngine) buff;
        BaseTool tool=eng.baseTool;
        Hand hand=tool.hand;
        Item toolItem=tool.toolItem;
        Item ammoItem=currentAmmo.GetCurrentAmmo(toolItem);
        Vector3 firePos=hand.GetHoldingToolPosi(ToolPosition.silencePos);
        Vector3 targetPos=EventCenter.WorldCenter.GetParm<Vector3>(nameof(EventNames.GetMouseLookAt));
        /*
        bullet excel���ݣ�
itemID	level	//������������
����	�ӵ�prefabs	���	�ٶ�	�˺�	0-4
��ͷ����	�˺�����	��Ϣɢ�ʣ�10m��/m	����ɢ������	ɢ��ÿ�����	5-9
��͸��	֧�ֵ�tag   10,11


        tool excel����
itemID	level	//������������
����	Ԥ����	ģ����	��������	����	0-4
�˺�	���	װ��ʱ��	��Ϣɢ�ʣ�10m��/m	����ɢ������	5-9
ɢ��ÿ�����	��������    10��11
����1	����2	����3	����4	����5	12-16		

        */
        /*
�˺�����ҩ����+��������
�˺��ӳ�+��hand�ϵ�<key=dam+,float>buff�޸�
�˺��ӳ�%��hand�ϵ�<key=dam%,float>buff�޸�
�����ʣ�hand�ϵ�<key=crit,float>buff�޸�
�����ӳɣ�hand�ϵ�<key=crit+%,float>buff�޸�
�ӵ�Ԥ�裺��ҩ���û�hand�ϵ�<key=bulletprefabs,str>buff�޸�
�˺����ͣ���ҩ���û�hand�ϵ�<key=tooldamtype,str>buff�޸�
�������ͣ���������
���ֵ�Ԫ����ҩ����
��̣���ҩ����+��������
����ʱ�䣺��ҩ����
�ٶȣ���ҩ����
��ǰɢ�ʣ�basetool��dic<spread,fp>
ɢ����״����ҩ����+��������
����λ�ã�Ĭ����ģ�͡�������λ��
Ŀ��λ�ã�Ĭ�ϡ����ָ��λ�á�
��������:��ҩitem��<tag,obj>+hand�ϵ�<key=tooldambuff,buffnamestr>buff
        */
        Dictionary<string,FP> bulletexcel=(Dictionary<string,FP>)(bulletValue.Get(caster,target[0],buff,new object[]{ammoItem}).data);
        Dictionary<string, FP> toolexcel =(Dictionary<string, FP>)(valueSource.Get(caster,target[0],buff,new object[]{toolItem}).data);

        int bulletCount=bulletexcel["��ͷ����"];
        float damage=bulletexcel["�˺�"].Convert<float>()+toolexcel["�˺�"];

        
        float damAdd=0;
        float damPercent=0;
        float crit=0;
        float CritAdd=0;  

        FP temp=hand.GetBuffedValue(HandDataName.damAdd);
        if(temp!=null)damAdd=temp;
        temp=hand.GetBuffedValue(HandDataName.damPercent);
        if(temp!=null)damPercent=temp;
        temp=hand.GetBuffedValue(HandDataName.crit);
        if(temp!=null)crit=temp;
        temp=hand.GetBuffedValue(HandDataName.CritAdd);
        if(temp!=null)CritAdd=temp;

        string bulletprefab=bulletexcel["Ԥ����"];
        DamageType dt=bulletexcel["�˺�����"].Convert<DamageType>();
        ToolType tt=toolexcel["��������"].Convert<ToolType>();
        PhyBullet pb=null;//phybullet�ı���������δȷ��
        float range=bulletexcel["���"];
        float speed=bulletexcel["�ٶ�"].Convert<float>();
         float exitTime=range/speed;
        float spread=tool.GetData(BaseToolDataName.spread);

        Shape2D spreadShape=Shape2D.circle;  
        //Vector3 firepos=;
        //Vector3 targetpos;
        Dictionary<string,object> exd=new Dictionary<string, object>();
        Dictionary<ItemContent,object> bulletExd=ammoItem.exd;
        if(bulletExd!=null)
        foreach (var item in bulletExd)
        {
            exd.Add(item.Key.ToString(),item.Value);
        }
        temp=hand.GetBuffedValue(HandDataName.damBuff);
        if(temp!=null)
        {
            exd.Add(nameof(HandDataName.damBuff),temp);
        }


        parm.num=bulletCount;
        parm.dam.value=damage;
        parm.dam.AdditionV=damAdd;
        parm.dam.AdditionPercent=damPercent;
        parm.dam.CritPercent=crit;
        parm.dam.CritAdd=CritAdd;

        parm.dam.type=dt;
        parm.sourceUUID=caster.UUID;
        parm.tool=tool;
        
        parm.speed=speed;
        parm.range=range;
        parm.exittime=exitTime;
        parm.spreadRadius=spread;
        parm.shape=spreadShape;
        parm.type=bulletprefab;
        parm.parms=exd;
        
        parm.pos=firePos;
        parm.targetPos=targetPos;

        //�����ӵ�
        EventCenter.WorldCenter.SendEvent<BulletParm>(nameof(EventNames.FireBullet), parm);
    }
    


    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}

public class ToolCurrentAmmo
{
 
    public Item GetCurrentAmmo(Item toolItem)
    {
        object o = toolItem.GetContent(ItemContent.AmmoItemList);
        if (o != null)
        {
            Item[] ammos = (Item[])o;
            if (ammos.Length == 0) return Item.Empty;
            if (ammos.Length == 1)
            {
                return ammos[0];
            }
            int ind = (int)toolItem.GetContent(ItemContent.CurrentAmmoIndex);
            if (ind < 0 || ind >= ammos.Length) { Debug.LogError("index>ammo����"); return Item.Empty; }
            return ammos[ind];
        }
        else return Item.Empty;
    }
}