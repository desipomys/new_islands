using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public enum CharacterEventName
{
    entity_type, entity_hp, entity_food, entity_pow,entity_tired,
    entity_maxHp, entity_maxFood, entity_maxPow, entity_def,
    entity_spd, entity_runspd, entity_powUpRate, entity_hpUpRate,

    armorH,armorTD,

    skill_pow, skill_know, skill_agi, skill_phy, skill_blade, skill_hammer,
    skill_bow, skill_rifle, skill_auto, skill_explo, skill_fire, skill_ice,
    skill_electro, skill_radio, skill_posion, skill_corrupt, skill_necroman,
    skill_heal, skill_build
}

public class CharacterEntity : MonoBehaviour,IEventRegister,IDataContainer
{
    public bool isCheat=false;
   public Charactor_Data char_data=new Charactor_Data();//最大值和固定值
    
   public Charactor_Skill_Data skill=new Charactor_Skill_Data();
    EventCenter center;

    ValueChangeParm<float> healtharg=new ValueChangeParm<float>();
    ValueChangeParm<float> powarg = new ValueChangeParm<float>();
    ValueChangeParm<float> foodarg = new ValueChangeParm<float>();

    /// <summary>
    /// 向worldcenter发送的伤害或死亡事件参数
    /// </summary>
    /// <returns></returns>
    EntityDamageEventArg damageEvent=new EntityDamageEventArg();

    public virtual int GetDataCollectPrio => 0;

    public virtual void OnEventRegist(EventCenter e)
    {
        center=e;
        //e.RegistFunc<long>("getUUID",()=>{return e.UUID;});

        #region char_data get
        e.RegistFunc<EntityType>("entity_type",()=>{return char_data.type;});

        e.RegistFunc<float>("entity_hp",()=>{return char_data.health;});
        e.RegistFunc<float>("entity_food",()=>{return char_data.food;});
        e.RegistFunc<float>("entity_pow",()=>{return char_data.power;});

        e.RegistFunc<bool>(nameof(CharacterEventName.entity_tired), () => { return char_data.isTired; });

        e.RegistFunc<float>("entity_maxHp",()=>{return char_data.maxHealth;});
        e.RegistFunc<float>("entity_maxFood",()=>{return char_data.maxFood;});
        e.RegistFunc<float>("entity_maxPow",()=>{return char_data.maxPower;});
        e.RegistFunc<float>("entity_def",()=>{return char_data.def;});
        e.RegistFunc<float>("entity_spd",()=>{return char_data.speed;});
        e.RegistFunc<float>("entity_runspd",()=>{return isCheat?10*char_data.runSpeed:char_data.runSpeed;});

        e.RegistFunc<float>("entity_powUpRate",()=>{return char_data.basePowerUpRate;});
        e.RegistFunc<float>("entity_hpUpRate",()=>{return char_data.baseHealthUpRate;});

        e.RegistFunc<short>("skill_pow",()=>{return skill.power;});
        e.RegistFunc<short>("skill_know",()=>{return skill.knowled;});
        e.RegistFunc<short>("skill_agi",()=>{return skill.agile;});
        e.RegistFunc<short>("skill_phy",()=>{return skill.physi;});

        e.RegistFunc<short>("skill_blade",()=>{return skill.bladeSkill;});
        e.RegistFunc<short>("skill_hammer",()=>{return skill.hammerSkill;});
        e.RegistFunc<short>("skill_bow",()=>{return skill.bowSkill;});
        e.RegistFunc<short>("skill_rifle",()=>{return skill.rifleSkill;});
        e.RegistFunc<short>("skill_auto",()=>{return skill.autoSkill;});
        e.RegistFunc<short>("skill_explo",()=>{return skill.exploSkill;});

        e.RegistFunc<short>("skill_fire",()=>{return skill.fireSkill;});
        e.RegistFunc<short>("skill_ice",()=>{return skill.iceSkill;});
        e.RegistFunc<short>("skill_electro",()=>{return skill.electroSkill;});
        e.RegistFunc<short>("skill_radio",()=>{return skill.radioSkill;});
        e.RegistFunc<short>("skill_posion",()=>{return skill.posionSkill;});
        e.RegistFunc<short>("skill_corrupt",()=>{return skill.corruptionSkill;});
        e.RegistFunc<short>("skill_necroman",()=>{return skill.necromanSkill;});
        e.RegistFunc<short>("skill_heal",()=>{return skill.healSkill;});
        e.RegistFunc<short>("skill_build",()=>{return skill.buildSkill;});

        e.RegistFunc<bool>("isCheat", () => { return isCheat; });
        #endregion

        #region char_data set

        e.ListenEvent<EntityType>("set_entity_type", (a) => {  char_data.type=a; });

        e.ListenEvent<float>("set_entity_hp", (a) => { char_data.health=a; });
        e.ListenEvent<float>("set_entity_food", (a) => {  char_data.food=a; });
        e.ListenEvent<float>("set_entity_pow", (a) => {  char_data.power=a; });

        e.ListenEvent<float>("set_entity_maxHp", (a) => {  char_data.maxHealth=a; });
        e.ListenEvent<float>("set_entity_maxFood", (a) => {  char_data.maxFood=a; });
        e.ListenEvent<float>("set_entity_maxPow", (a) => {  char_data.maxPower=a; });
        e.ListenEvent<float>("set_entity_def", (a) => {  char_data.def=a; });
        e.ListenEvent<float>("set_entity_spd", (a) => {  char_data.speed=a; });
        e.ListenEvent<float>("set_entity_runspd", (a) => {  char_data.runSpeed=a; });

        e.ListenEvent<float>("set_entity_powUpRate", (a) => {  char_data.basePowerUpRate=a; });
        e.ListenEvent<float>("set_entity_hpUpRate", (a) => {  char_data.baseHealthUpRate=a; });

        e.ListenEvent<short>("set_skill_pow", (a) => {  skill.power=a; });
        e.ListenEvent<short>("set_skill_know", (a) => {  skill.knowled=a; });
        e.ListenEvent<short>("set_skill_agi", (a) => {  skill.agile=a; });
        e.ListenEvent<short>("set_skill_phy", (a) => {  skill.physi=a; });

        e.ListenEvent<short>("set_skill_blade", (a) => {  skill.bladeSkill=a; });
        e.ListenEvent<short>("set_skill_hammer", (a) => {  skill.hammerSkill=a; });
        e.ListenEvent<short>("set_skill_bow", (a) => {  skill.bowSkill=a; });
        e.ListenEvent<short>("set_skill_rifle", (a) => {  skill.rifleSkill=a; });
        e.ListenEvent<short>("set_skill_auto", (a) => {  skill.autoSkill=a; });
        e.ListenEvent<short>("set_skill_explo", (a) => {  skill.exploSkill=a; });

        e.ListenEvent<short>("set_skill_fire", (a) => {  skill.fireSkill=a; });
        e.ListenEvent<short>("set_skill_ice", (a) => {  skill.iceSkill=a; });
        e.ListenEvent<short>("set_skill_electro", (a) => {  skill.electroSkill=a; });
        e.ListenEvent<short>("set_skill_radio", (a) => {  skill.radioSkill=a; });
        e.ListenEvent<short>("set_skill_posion", (a) => {  skill.posionSkill=a; });
        e.ListenEvent<short>("set_skill_corrupt", (a) => {  skill.corruptionSkill=a; });
        e.ListenEvent<short>("set_skill_necroman", (a) => {  skill.necromanSkill=a; });
        e.ListenEvent<short>("set_skill_heal", (a) => {  skill.healSkill=a; });
        e.ListenEvent<short>("set_skill_build", (a) => {  skill.buildSkill=a; });

        e.ListenEvent<bool>("set_isCheat", (a) => { isCheat = a; });
        #endregion

        e.ListenEvent(nameof(PlayerEventName.onRespawn), Respawn);
        e.ListenEvent<Damage,EventCenter,BaseTool>(nameof(PlayerEventName.onDamage),OnDamage);//默认优先级0，buff减伤可以用大于0的优先级以提前接收damage并修改damage
        e.ListenEvent<Movement_Stat, float, DIR>(nameof(PlayerEventName.move),OnMove);
    }
    public virtual void AfterEventRegist()
    {

    }

    //void setUUID(long uu){UUID=uu;}

    public bool IsDead(){return char_data.health<=0;}

    public void OnMove(Movement_Stat stat, float speed, DIR dir)
    {
        if(stat.Contain(Movement_Stat.run))
        {
            OnUsePower(0.5f);
        }
        else { OnUsePower(0.25f); Debug.Log("用力"); }
    }
    public void FixedUpdate()
    {
        
        OnUsePower(-0.5f);
    }

    public void OnDamage(Damage d,EventCenter source,BaseTool tool)
    {//传入damage在运算过程中不许修改
        
       float f= DamageLogic.ApplyDamage(d,source,tool,center);
       HealthChange(f,source,tool,d);
        if(d.IsContainBuff())
            DamageLogic.ApplyBuff((BaseBuff[])d.exd["buff"],source,center);
    }
    public void HealthChange(float damage,EventCenter source,BaseTool tool,Damage du)
    {
        if (IsDead()) return;
        healtharg.old=center.GetParm<float>("entity_hp");
        healtharg.max=center.GetParm<float>("entity_maxHp");
        if(healtharg.old-damage<=0)
        {
            char_data.health = 0;
            healtharg.now=0;
            center.SendEvent<ValueChangeParm<float>,EventCenter,BaseTool,Damage>(nameof(PlayerEventName.onHealthChangeBy),healtharg,source,tool,du);
            
            damageEvent.Set(healtharg,source,center,tool,du);
            EventCenter.WorldCenter.SendEvent<EntityDamageEventArg>(nameof(PlayerEventName.onHealthChangeBy),damageEvent);
            
           
            Die(damage,source,tool,du);//killby事件
        }
        else
        {
            
            char_data.health-=damage;
            healtharg.now=center.GetParm<float>("entity_hp");
            center.SendEvent<ValueChangeParm<float>,EventCenter,BaseTool,Damage>(nameof(PlayerEventName.onHealthChangeBy),healtharg,source,tool,du);
            
            damageEvent.Set(healtharg,source,center,tool,du);
            EventCenter.WorldCenter.SendEvent<EntityDamageEventArg>(nameof(PlayerEventName.onHealthChangeBy),damageEvent);
        }
    }
    public void Die(float damage,EventCenter source,BaseTool tool,Damage du)
    {

        center.SendEvent<float,EventCenter,BaseTool,Damage>(nameof(PlayerEventName.onDie),damage,source,tool,du);

        healtharg.old=damage;
        healtharg.now=damage;
        damageEvent.Set(healtharg,source,center,tool,du);
        EventCenter.WorldCenter.SendEvent<EntityDamageEventArg>(nameof(PlayerEventName.onDie),damageEvent);
    }

    public void OnUsePower(float value)
    {
        powarg.old= center.GetParm<float>("entity_pow");
        powarg.max= center.GetParm<float>("entity_maxPow");
        if (powarg.old - value <= 0)
        {
            char_data.power = 0;
            char_data.isTired = true;
            powarg.now = 0;
            center.SendEvent<ValueChangeParm<float>>(nameof(PlayerEventName.onPowerChange), powarg);
        }
        else
        {
            char_data.power=Mathf.Clamp(char_data.power-value,0,char_data.maxPower);
            if (char_data.power > 10) char_data.isTired = false;
            powarg.now = center.GetParm<float>("entity_pow");
            center.SendEvent<ValueChangeParm<float>>(nameof(PlayerEventName.onPowerChange), powarg);
        }
    }

    public void Update()
    {
        
    }
    public void ChangeData(CharacterEventName name,float value)
    {
        char_data.SetData(name, value);

    }
    public void ChangeData(CharacterEventName[] names,float[] values)
    {
        int min = Mathf.Min(names.Length, values.Length);
        for (int i = 0; i < min; i++)
        {
            char_data.SetData(names[i], values[i]);
        }
        //发更新事件
    }

    public void Respawn()
    {
        char_data.health = char_data.maxHealth;
        char_data.power = char_data.maxPower;
        healtharg.old = char_data.health;
        healtharg.max = char_data.maxHealth;
        healtharg.now= char_data.health;
        powarg.old = char_data.power;
        powarg.max = char_data.maxPower;
        powarg.now = char_data.power;

        center.SendEvent<ValueChangeParm<float>, EventCenter, BaseTool, Damage>(nameof(PlayerEventName.onHealthChangeBy), healtharg,null,null,null);
        center.SendEvent<ValueChangeParm<float>>(nameof(PlayerEventName.onPowerChange), powarg);

        
    }

    public object ToObject()
    {
        object[] temp=new JObject[]{ JObject.FromObject(char_data), JObject.FromObject(skill) };
        return JArray.FromObject(temp);
    }
    public void FromObject(object data)
    {
        JObject[] temp=((JArray)data).ToObject<JObject[]>();
        char_data=Charactor_Data.FromString(temp[0].ToString());
        skill=Charactor_Skill_Data.FromString(temp[1].ToString());
        //UUID=long.Parse(temp[2]);
        if (IsDead()) Die(0,null,null,null);//不要跟被杀事件混在一起
    }
}
